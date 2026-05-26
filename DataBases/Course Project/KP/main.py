import sys
import yaml
from config import *
from MagazineUser import User

from PyQt5 import QtWidgets
from PyQt5.QtWidgets import QListWidgetItem, QTableWidgetItem
from PyQt5.QtGui import QPixmap

import AuthGui
import DeleteGui
import EditGui
import AddGui
import Gui
import ModelInfoGui
import BrandInfoGui

import psycopg2
from psycopg2 import Error

try:
    # Подключение к существующей базе данных
    connection = psycopg2.connect(dbname=DBNAME, user=USER,
                                  # пароль, который указали при установке PostgreSQL
                                  password=PASSWORD,
                                  host=HOST,
                                  port=PORT)

    # Курсор для выполнения операций с базой данных
    cursor = connection.cursor()
    # Распечатать сведения о PostgreSQL
    print("Информация о сервере PostgreSQL")
    print(connection.get_dsn_parameters(), "\n")
    # Выполнение SQL-запроса
    cursor.execute("SELECT version();")
    # Получить результат
    record = cursor.fetchall()
    print("Вы подключены к - ", record, "\n")
except (Exception, Error) as error:
    print("Ошибка при работе с PostgreSQL", error)


# region
class BrandWindow(QtWidgets.QMainWindow, Gui.Ui_MainWindow):

    def __init__(self, cursor):
        super().__init__()
        self.cur = cursor
        self.setupUi(self)
        self.setFixedSize(800, 600)

        global law
        if not law:
            self.AddButton.setVisible(False)

        self.set_brands_list()

        self.AddButton.clicked.connect(self.add_window)
        self.NextButton.clicked.connect(self.next_window)
        self.BackButton.clicked.connect(self.auth_window)
        self.ListWidget.clicked.connect(self.get_brand)
        self.InfoButton.clicked.connect(self.info_window)

    def add_window(self):
        dlg = BrandAddDialog(self.cur)
        dlg.exec()

        global window
        window = BrandWindow(self.cur)
        window.show()

    def auth_window(self):
        global window
        window = AuthWindow(self.cur)
        window.show()

    def info_window(self):
        global window
        window = BrandInfoWindow(self.cur, self.NameLabel.text())
        window.show()

    def next_window(self):
        global window
        window = ModelWindow(self.cur, self.NameLabel.text())
        window.show()

    def set_brands_list(self):
        self.cur.execute("SELECT name FROM companies ORDER BY name;")
        res = [i[0] for i in self.cur.fetchall()]
        for i in res:
            item = QListWidgetItem(i)
            self.ListWidget.addItem(item)

        self.set_brand(res[0])

    def set_brand(self, brand):
        self.NameLabel.setText(brand)
        self.cur.execute(f'select logo from companies where name = \'{brand}\'')
        file_path = self.cur.fetchone()[0]
        pixmap = QPixmap()
        pixmap.load(f"../{file_path}")
        self.PicLabel.setPixmap(pixmap)

    def get_brand(self):
        brand = self.ListWidget.currentItem().text()
        self.set_brand(brand)


class ModelWindow(QtWidgets.QMainWindow, Gui.Ui_MainWindow):
    def __init__(self, cursor, brand):
        super().__init__()
        self.brand = brand
        self.cur = cursor
        self.setupUi(self)
        self.setFixedSize(800, 600)
        global law
        if not law:
            self.AddButton.setVisible(False)
        self.AddButton.clicked.connect(self.add_window)
        self.NextButton.setVisible(False)
        self.InfoButton.setGeometry(110, 380, 135, 60)
        self.BackButton.clicked.connect(self.back_window)
        self.NameLabel.setText("Название Модели")
        self.set_models_list()
        self.ListWidget.clicked.connect(self.get_model)
        self.InfoButton.clicked.connect(self.info_window)

    def add_window(self):
        dlg = ModelAddDialog(self.cur, self.brand)
        dlg.exec()
        global window

        window = ModelWindow(self.cur, self.brand)
        window.show()

    def info_window(self):
        self.cur.execute(f'SELECT * from specification '
                         f'WHERE model_id = (SELECT id from models_range where model_name = \'{self.NameLabel.text()}\')')
        res = self.cur.fetchall()
        if len(res) == 0:
            self.NameLabel.setText('Пустая запись')
        if self.NameLabel.text() not in ['Нет моделей', 'Пустая запись']:
            global window

            window = ModelInfoWindow(self.cur, self.brand, self.NameLabel.text())
            window.show()

    def get_model(self):
        model = self.ListWidget.currentItem().text()
        self.set_model(model)

    def back_window(self):
        global window

        window = BrandWindow(self.cur)
        window.show()

    def set_models_list(self):
        # todo обработка ситуации, когда нет моделей
        self.cur.execute(f'select model_name from models_range'
                         f' where company_id = (select id from companies where name = \'{self.brand}\')')

        res = [i[0] for i in self.cur.fetchall()]
        res.sort()
        if len(res):
            for i in res:
                item = QListWidgetItem(i)
                self.ListWidget.addItem(item)
            self.set_model(res[0])
        else:
            self.NameLabel.setText('Нет моделей')

    def set_model(self, model):
        self.NameLabel.setText(model)


class ModelInfoWindow(QtWidgets.QMainWindow, ModelInfoGui.Ui_MainWindow):
    def __init__(self, cursor, brand, model):
        super().__init__()
        self.setupUi(self)
        self.setFixedSize(800, 600)
        global law
        if not law:
            self.EditButton.setVisible(False)
            self.DeletButton.setVisible(False)
        self.EditButton.clicked.connect(self.edit_window)
        self.DeletButton.clicked.connect(self.delete)
        self.cur = cursor
        self.BrandNameLabel.setText(brand)
        self.ModelNameLabel.setText(model)
        self.BackButton.clicked.connect(self.back_window)
        self.BrandInfoButton.clicked.connect(self.brand_info_window)
        self.set_info_table()
        self.SpecTable.setEditTriggers(QtWidgets.QAbstractItemView.NoEditTriggers)

    def delete(self):
        dlg = DeleteDialog()
        dlg.exec()
        if dlg.res:
            self.cur.execute(f'DELETE FROM models_range WHERE model_name = \'{self.ModelNameLabel.text()}\'')
            self.back_window()

    def edit_window(self):
        dlg = ModelEditDialog(self.cur, self.BrandNameLabel.text(), self.ModelNameLabel.text())
        dlg.exec()

        global window
        window = ModelInfoWindow(self.cur, self.BrandNameLabel.text(), self.ModelNameLabel.text())
        window.show()

    def back_window(self):
        global window

        window = ModelWindow(self.cur, self.BrandNameLabel.text())
        window.show()

    def brand_info_window(self):
        global window

        window = BrandInfoWindow(self.cur, self.BrandNameLabel.text())
        window.show()

    def set_info_table(self):
        self.cur.execute("SELECT column_name FROM information_schema.columns WHERE table_name = 'specification';")
        res = [i[0] for i in self.cur.fetchall()]
        self.cur.execute(f'SELECT * from specification '
                         f'WHERE model_id = (SELECT id from models_range where model_name = \'{self.ModelNameLabel.text()}\')')
        self.SpecTable.setColumnCount(2)
        self.SpecTable.setRowCount(len(res))
        self.SpecTable.setHorizontalHeaderLabels(['Параметры', 'Значения'])
        for i in range(len(res)):
            self.SpecTable.setItem(i, 0, QTableWidgetItem(res[i]))
        res2 = self.cur.fetchall()
        if len(res2):
            res2 = list(res2[0])
            for i in range(len(res2)):
                res2[i] = str(res2[i])
            for i in range(len(res)):
                self.SpecTable.setItem(i, 1, QTableWidgetItem(res2[i]))


class BrandInfoWindow(QtWidgets.QMainWindow, BrandInfoGui.Ui_MainWindow):
    def __init__(self, cursor, brand):
        super().__init__()
        self.cur = cursor
        self.setupUi(self)
        self.setFixedSize(800, 600)
        global law
        if not law:
            self.EditButton.setVisible(False)
            self.DeleteButton.setVisible(False)
        self.EditButton.clicked.connect(self.edit_window)
        self.DeleteButton.clicked.connect(self.delete)
        self.BrandNameLabel.setText(brand)
        self.BackButton.clicked.connect(self.back_window)
        self.NextButton.clicked.connect(self.next_window)
        self.set_info_table()
        self.InfoTable.setEditTriggers(QtWidgets.QAbstractItemView.NoEditTriggers)

    def delete(self):
        dlg = DeleteDialog()
        dlg.exec()
        if dlg.res:
            self.cur.execute(f'DELETE FROM companies WHERE name = \'{self.BrandNameLabel.text()}\'')
            self.back_window()

    def edit_window(self):
        dlg = BrandEditDialog(self.cur, self.BrandNameLabel.text())
        dlg.exec()
        global window
        window = BrandInfoWindow(self.cur, dlg.brand)
        window.show()

    def back_window(self):
        global window

        window = BrandWindow(self.cur)
        window.show()

    def next_window(self):
        global window

        window = ModelWindow(self.cur, self.BrandNameLabel.text())
        window.show()

    def complete(self):
        dct = dict()
        for i in range(self.SpecTable.rowCount()):
            dct[self.SpecTable.item(i, 0).text()] = self.SpecTable.item(i, 1).text()

        end_prod = dct.get('end_of_production', 'NULL')
        if end_prod != '':
            end_prod = f'\'{end_prod}\''
        else:
            end_prod = 'NULL'

        self.cur.execute(f"INSERT INTO models_range(company_id, model_name) SELECT id, \'{dct['model_name']}\' "
                         f"from companies where name = \'{self.brand}\';")
        self.cur.execute(f"INSERT INTO "
                         f"specification(model_id, generation, start_of_production, end_of_production, "
                         f"engine, engine_displacement, HP, body_type)"
                         f" WITH model AS (SELECT id from models_range WHERE model_name = \'{dct['model_name']}\') "
                         f"SELECT model.id, \'{dct['generation']}\', \'{dct['start_of_production']}\', {end_prod}, \'{dct['engine']}\',"
                         f" {dct['engine_displacement']}, {dct['hp']}, {dct['body_type']} from model;")
        self.close()

    def set_info_table(self):
        dct = dict()

        self.cur.execute("SELECT column_name FROM information_schema.columns WHERE table_name = 'companies' "
                         "ORDER BY ordinal_position;")
        res = [i[0] for i in self.cur.fetchall()]

        self.cur.execute(f'SELECT * FROM companies where name = \'{self.BrandNameLabel.text()}\';')
        res2 = list(map(str, self.cur.fetchall()[0]))
        self.InfoTable.setColumnCount(2)
        self.InfoTable.setRowCount(len(res))
        self.InfoTable.setHorizontalHeaderLabels(['Параметры', 'Значения'])

        for i in range(len(res)):
            self.InfoTable.setItem(i, 0, QTableWidgetItem(res[i]))
            self.InfoTable.setItem(i, 1, QTableWidgetItem(res2[i]))


class AuthWindow(QtWidgets.QMainWindow, AuthGui.Ui_MainWindow):
    def __init__(self, cursor):
        super().__init__()
        self.setupUi(self)
        self.setFixedSize(800, 600)

        # Connect funtions to buttons
        self.AuthAuthButton.clicked.connect(self.auth_handle)
        self.AuthRegButton.clicked.connect(self.open_registration_widget)
        self.RegRegButton.clicked.connect(self.reg_handle)

        self.cur = cursor

    def auth_handle(self):
        login = self.LoginAuthLine.text()
        password = self.PasswordAuthLine.text()
        # password = hash(self.PasswordAuthLine.text())

        self.cur.execute(f'SELECT category from auth.users where login = \'{login}\' and password = \'{password}\'')
        user_type = self.cur.fetchone()
        if user_type:
            user = User(login, user_type[0])
            if user_type[0] == 'user':
                global law
                law = 0
            else:
                law = 1
            self.start_brand_window()
        else:
            self.NotificationAuth.setText('Неверные данные')

    def reg_handle(self):
        login = self.LoginRegLine.text()
        password = self.PasswordRegLine.text()
        # password = hash(self.PasswordRegLine.text())

        try:
            print(login, password, sep='____')
            self.cur.execute(
                f'INSERT INTO auth.users (login, password, category) VALUES (\'{login}\', \'{password}\', \'user\');'
            )
            user = User(login, 'user')
            global law
            law = 0
            self.NotificationReg.setText("")
            self.start_brand_window()

        except Exception as e:
            self.NotificationReg.setText("Такой пользователь уже зарегистрирован")
            self.cur.execute("ROLLBACK;")
            print("Ошибка при работе с PostgreSQL", e)

    def start_brand_window(self):
        global window

        window = BrandWindow(self.cur)
        window.show()

    def open_registration_widget(self):
        self.regWidget.setVisible(True)
        self.authWidget.setVisible(False)

        # transferring data from the authorization window
        self.LoginRegLine.setText(self.LoginAuthLine.text())
        self.PasswordRegLine.setText(self.PasswordAuthLine.text())


class BrandAddDialog(QtWidgets.QDialog, AddGui.Ui_Dialog):
    def __init__(self, cursor):
        super().__init__()
        self.setupUi(self)
        self.cur = cursor
        self.BackButton.clicked.connect(self.back)
        self.CompleteButton.clicked.connect(self.complete)
        self.set_table()

    def set_table(self):
        self.cur.execute("SELECT column_name FROM information_schema.columns WHERE table_name = 'companies';")
        res = [i[0] for i in self.cur.fetchall()]
        res.pop(0)
        self.SpecTable.setColumnCount(2)
        self.SpecTable.setRowCount(len(res))
        self.SpecTable.setHorizontalHeaderLabels(['Параметры', 'Значения'])
        for i in range(len(res)):
            self.SpecTable.setItem(i, 0, QTableWidgetItem(res[i]))
            self.SpecTable.setItem(i, 1, QTableWidgetItem(''))

    def back(self):
        self.close()

    def complete(self):
        lst = list()
        for i in range(self.SpecTable.rowCount()):
            lst.append(self.SpecTable.item(i, 1).text())
        self.cur.execute("SELECT column_name FROM information_schema.columns WHERE table_name = 'companies';")
        res = [i[0] for i in self.cur.fetchall()]
        res.pop(0)
        res.pop()
        self.cur.execute(f"INSERT INTO companies ({', '.join(res)}) VALUES"
                         f" (\'{lst[0]}\', \'{lst[1]}\', \'{lst[2]}\', \'{lst[3]}\');")
        self.close()


class ModelAddDialog(QtWidgets.QDialog, AddGui.Ui_Dialog):
    def __init__(self, cursor, brand):
        super().__init__()
        self.setupUi(self)
        self.cur = cursor
        self.brand = brand
        self.BackButton.clicked.connect(self.back)
        self.CompleteButton.clicked.connect(self.complete)
        self.set_table()

    def set_table(self):
        self.cur.execute("SELECT column_name FROM information_schema.columns WHERE table_name = 'specification';")
        res = [i[0] for i in self.cur.fetchall()]
        dct = dict().fromkeys(res)
        dct.pop('id')
        dct.pop('model_id')

        keys = list(dct.keys())
        self.SpecTable.setColumnCount(2)
        self.SpecTable.setRowCount(len(keys) + 1)
        self.SpecTable.setHorizontalHeaderLabels(['Параметры', 'Значения'])
        self.SpecTable.setItem(0, 0, QTableWidgetItem("model_name"))
        for i in range(1, len(keys) + 1):
            self.SpecTable.setItem(i, 0, QTableWidgetItem(keys[i - 1]))
            self.SpecTable.setItem(i, 1, QTableWidgetItem(''))

    def back(self):
        self.close()

    def complete(self):
        dct = dict()
        for i in range(self.SpecTable.rowCount()):
            dct[self.SpecTable.item(i, 0).text()] = self.SpecTable.item(i, 1).text()

        end_prod = dct.get('end_of_production', 'NULL')
        if end_prod != '':
            end_prod = f'\'{end_prod}\''
        else:
            end_prod = 'NULL'

        self.cur.execute(f"INSERT INTO models_range(company_id, model_name) SELECT id, \'{dct['model_name']}\' "
                         f"from companies where name = \'{self.brand}\';")
        self.cur.execute(f"INSERT INTO "
                         f"specification(model_id, generation, start_of_production, end_of_production, "
                         f"engine, engine_displacement, HP, body_type)"
                         f" WITH model AS (SELECT id from models_range WHERE model_name = \'{dct['model_name']}\') "
                         f"SELECT model.id, \'{dct['generation']}\', \'{dct['start_of_production']}\', {end_prod}, \'{dct['engine']}\',"
                         f" {dct['engine_displacement']}, {dct['hp']}, {dct['body_type']} from model;")
        self.close()


class ModelEditDialog(QtWidgets.QDialog, EditGui.Ui_Dialog):
    def __init__(self, cursor, brand, model):
        super().__init__()
        self.setupUi(self)
        self.cur = cursor
        self.brand = brand
        self.model = model
        self.EditButton.clicked.connect(self.edit)
        self.BackButton.clicked.connect(self.back)
        self.set_table()
        self.SpecTable.setEditTriggers(QtWidgets.QAbstractItemView.NoEditTriggers)
        self.SpecTable.itemClicked.connect(self.handle_item_click)

    def handle_item_click(self, item):
        self.SpecNameLabel.setText(self.SpecTable.item(item.row(), 0).text())
        self.SpecLine.setText(self.SpecTable.item(item.row(), 1).text())

    def edit(self):
        self.cur.execute(f'SELECT id from specification '
                         f'WHERE model_id = (SELECT id from models_range where model_name = \'{self.model}\')')
        res = self.cur.fetchone()[0]
        self.cur.execute(
            f'UPDATE specification set {self.SpecNameLabel.text()} = \'{self.SpecLine.text()}\' where id = {res}')
        self.set_table()

    def back(self):
        self.close()

    def set_table(self):
        self.cur.execute("SELECT column_name FROM information_schema.columns WHERE table_name = 'specification';")
        res = [i[0] for i in self.cur.fetchall()]
        self.cur.execute(f'SELECT * from specification '
                         f'WHERE model_id = (SELECT id from models_range where model_name = \'{self.model}\')')
        res2 = list(self.cur.fetchall()[0])
        for i in range(len(res2)):
            res2[i] = str(res2[i])
        self.SpecTable.setColumnCount(2)
        self.SpecTable.setRowCount(len(res))
        self.SpecTable.setHorizontalHeaderLabels(['Параметры', 'Значения'])
        for i in range(len(res)):
            self.SpecTable.setItem(i, 0, QTableWidgetItem(res[i]))

            self.SpecTable.setItem(i, 1, QTableWidgetItem(res2[i]))


class BrandEditDialog(QtWidgets.QDialog, EditGui.Ui_Dialog):
    def __init__(self, cursor, brand):
        super().__init__()
        self.setupUi(self)
        self.cur = cursor
        self.brand = brand
        self.EditButton.clicked.connect(self.edit)
        self.BackButton.clicked.connect(self.back)
        self.set_table()
        self.SpecTable.setEditTriggers(QtWidgets.QAbstractItemView.NoEditTriggers)
        self.SpecTable.itemClicked.connect(self.handle_item_click)

    def handle_item_click(self, item):
        self.SpecNameLabel.setText(self.SpecTable.item(item.row(), 0).text())
        self.SpecLine.setText(self.SpecTable.item(item.row(), 1).text())

    def edit(self):
        self.cur.execute(f'SELECT id from companies '
                         f'WHERE name = \'{self.brand}\'')
        res = self.cur.fetchone()[0]

        self.cur.execute(
            f'UPDATE companies set {self.SpecNameLabel.text()} = \'{self.SpecLine.text()}\' where id = {res}')
        if self.SpecNameLabel.text() == 'name':
            self.brand = self.SpecLine.text()
        self.set_table()

    def back(self):
        self.close()

    def set_table(self):
        self.cur.execute("SELECT column_name FROM information_schema.columns WHERE table_name = 'companies';")
        res = [i[0] for i in self.cur.fetchall()]
        self.cur.execute(f'SELECT * FROM companies where name = \'{self.brand}\';')
        res2 = list(self.cur.fetchall()[0])
        for i in range(len(res2)):
            res2[i] = str(res2[i])
        self.SpecTable.setColumnCount(2)
        self.SpecTable.setRowCount(len(res))
        self.SpecTable.setHorizontalHeaderLabels(['Параметры', 'Значения'])
        for i in range(len(res)):
            self.SpecTable.setItem(i, 0, QTableWidgetItem(res[i]))
            self.SpecTable.setItem(i, 1, QTableWidgetItem(res2[i]))


class DeleteDialog(QtWidgets.QDialog, DeleteGui.Ui_Dialog):
    def __init__(self):
        super().__init__()
        self.setupUi(self)
        self.res = None
        self.YesButton.clicked.connect(self.yes)
        self.NoButton.clicked.connect(self.no)

    def yes(self):
        self.res = True
        self.close()

    def no(self):
        self.res = False
        self.close()


if __name__ == '__main__':
    app = QtWidgets.QApplication(sys.argv)  # Новый экземпляр QApplication

    law = None
    window = AuthWindow(cursor)
    window.show()  # Показываем окно
    app.exec_()
    connection.commit()
    cursor.close()
    connection.close()
