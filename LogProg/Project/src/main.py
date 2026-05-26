def main():
    from gedcom.element.individual import IndividualElement
    from gedcom.parser import Parser

    parents_str = set()
    sex = set()
    # Наше дерево
    file = "GBTree.ged"

    gedcom_parser = Parser()
    gedcom_parser.parse_file(file)
    root_child_elements = gedcom_parser.get_root_child_elements()

    for child_element in root_child_elements:
        if isinstance(child_element, IndividualElement):
            name, surname = child_element.get_name()
            parents = gedcom_parser.get_parents(child_element)
            father_name, father_surname = '', ''
            mother_name, mother_surname = '', ''

            for parent in parents:
                if parent.get_gender() == 'M':
                    father_name, father_surname = parent.get_name()
                else:
                    mother_name, mother_surname = parent.get_name()

            if child_element.get_gender() == "M":
                sex.add(f'sex("{name} {surname}", m).\n')
            else:
                sex.add(f'sex("{name} {surname}", f).\n')
            if not (father_surname == "" and father_name == "" and mother_surname == "" and mother_name == ""):
                parents_str.add(f'parent("{father_name} {father_surname}", "{name} {surname}").\n')
                parents_str.add(f'parent("{mother_name} {mother_surname}", "{name} {surname}").\n')

    # Создаём файл для записи правил в прологе
    fo = open("database.pl", mode="w", encoding="utf-8")
    for i in parents_str:
        fo.write(i)
    for j in sex:
        fo.write(j)
    fo.close()


if __name__ == "__main__":
    main()
