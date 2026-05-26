class BrandWindow(QtWidgets.QMainWindow, Gui.Ui_MainWindow):
    def __init__(self, cursor):
        super().__init__()
        self.cur = cursor
        self.setupUi(self)
        self.setFixedSize(800, 600)
        global law
        if not law:
            self.AddButton.setVisible(False)
        self.AddButton.clicked.connect(self.add_window)
        self.NextButton.clicked.connect(self.next_window)
        self.set_brands_list()
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