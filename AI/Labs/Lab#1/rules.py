from pyknow import *
import schema
from enum import Enum


class ActiveLeisure(Fact):
    status = Field(bool, default=False)


class CountVisitors(Fact):
    pass


class ChildrenAge(Fact):
    def age_cond__(x): return isinstance(x, int) and x > 0 and x <= 18
    age = Field(age_cond__, mandatory=True)


class InterestPlace(Fact):
    name = Field(str, mandatory=True)


class DayOfWeek(Fact):
    pass


class Weekday(Fact):
    status = Field(bool, mandatory=True)


class PhysicalActivity(Fact):
    status = Field(bool, mandatory=True)


class Exhibition(KnowledgeEngine):
    @Rule(OR(DayOfWeek("Суббота"), DayOfWeek("Воскресенье")))
    def weekend(self):
        self.declare(Fact("Выходной"))

    @Rule(DayOfWeek("Суббота"))
    def saturday(self):
        self.declare(Fact("Суббота"))

    @Rule(OR(DayOfWeek("Вторник"), DayOfWeek("Среда"), DayOfWeek("Четверг"), DayOfWeek("Пятница")))
    def weekday(self):
        self.declare(Fact("Будни"))

    @Rule(
        Fact("Будни"),
        InterestPlace("Площадь промышленности")
    )
    def future_flowers(self):
        name__ = "Флористическое путешествие «Будущее в цветах»"
        repr__ = """Это настоящее путешествие в мир цветочных ароматов и богатство флоры всех 89 регионов России: от каждого из них на выставке представлены цветочные композиции."""
        self.declare(Fact(name=name__, repr=repr__))

    @Rule(
        Fact("Будни"),
        OR(
            InterestPlace("Площадь промышленности"),
            InterestPlace("Павильон №1"),
            InterestPlace("Фонтан \"Дружба Народов\""),
            InterestPlace("Фонтан \"Каменный Цветок\"")
        )
    )
    def architectural_route(self):
        name__ = 'Архитектурный маршрут: история места'
        repr__ = """Эта экскурсия будет интересна тем, кто уже посетил все экспозиции выставки "Россия" и хочет погрузиться в детали: узнать больше об истории ВДНХ и изучить не только архитектуру павильонов выставки, но и советского времени (будут рассказывать о развитии архитектуры Советского Союза, можно будет ознакомиться с историей ВДНХ и историческим наследием павильонов)"""
        self.declare(Fact(name=name__, repr=repr__))

    @Rule(
        Fact("Выходной"),
        OR(
            InterestPlace("Музей кино"),
            InterestPlace("Сочинская горка"),
            InterestPlace("Фонтан \"Дружба Народов\""),
            InterestPlace("Павильон Космос"),
            InterestPlace("Москвариум"),
            InterestPlace("Павильон Атом"),
            InterestPlace("Техноград")
        )
    )
    def active_route_charging(self):
        name__ = 'Активная экскурсия "На зарядку становись!"'
        repr__ = """Профессиональные тренеры проведут с участниками экскурсии утреннюю зарядку-тренировку на живописных локациях выставки “Россия”, связанных с космосом, технологиями, здоровьем, семьей, и спортом. В зависимости от локации гости выполнят «зарядку дружбы народов», «атомную зарядку», «космическую», «кино-зарядку» и т.д. Движения и правила их выполнения в каждой точке маршрута будут разными."""
        self.declare(Fact(name=name__, repr=repr__))

    @Rule(
        Fact("Выходной"),
        PhysicalActivity(True),
        OR(
            InterestPlace("Аллея славы"),
            InterestPlace("Запрудная зона")
        )
    )
    def active_route_walk(self):
        name__ = 'Активная экскурсия "Больше, чем прогулка!"'
        repr__ = """Всем любителям скандинавской ходьбы эта экскурсия дает возможность пройти по локациям выставки вместе с инструктором. Сертифицированный мастер познакомит с методикой скандинавской ходьбы и расскажет о ее достоинствах и оздоровительном эффекте."""
        self.declare(Fact(name=name__, repr=repr__))

    @Rule(
        OR(
            InterestPlace("Дом молодёжи"),
            InterestPlace("Площадь промышленности"),
            InterestPlace("Павильон Атом"),
            InterestPlace("Фонтан \"Каменный цветок\""),
            InterestPlace("Техноград"),
            InterestPlace("Галерея достижений")
        )
    )
    def night_quest(self):
        name__ = 'Вечерний квест "Город будущего, или в поисках ВДНХновения"'
        repr__ = """Ночь, улица, фонарь, ВДНХ… Экскурсия проходит в формате квеста в ночное время, когда солнце заходит за горизонт и темные аллеи парка освещены светом луны. В таких красках все выглядит иначе: более таинственно и захватывающе. Маршрут предполагает выполнение увлекательных заданий в разных локациях выставки и точно понравится любителям острых ощущений."""
        self.declare(Fact(name=name__, repr=repr__))

    @Rule(
        OR(
            InterestPlace("Павильон №1 (Движение первых)"),
            InterestPlace("Фонтан \"Дружбы народов\""),
            InterestPlace("Фонтан \"Каменный цветок\""),
            InterestPlace("Павильон Космос"),
            InterestPlace("Дом молодёжи")
        )

    )
    def media_route(self):
        name__ = 'Медиа маршрут "Не опубликован - Не было!"'
        repr__ = """Дети и подростки всецело погружены в виртуальную жизнь! Блогеры, лидеры мнений, стримеры! А кто бы сам не хотел оказаться на их месте? Но тот, кто уже пытался, 100% вам ответит, что на самом деле – это упорный труд! Постоянное обучение, тренировка навыков, не говоря уже о психологической выдержке и самодисциплине. Приготовьтесь! Этот маршрут полностью поменяет правила игры! Мы дадим вам готовые кейсы и лайфхаки, а если вы не знаете значения этих слов, с радостью переведем их на русский язык! Наши экскурсоводы расскажут, где брать вдохновение, как не опустить руки до первой тысячи подписчиков и помогут вам создать ваш первых контент! Вы увидите, насколько это процесс может быть сложным и интересным одновременно! Одной из самых интересных фишек нашего маршрута станет специальная движущаяся платформа с вращающейся камерой!"""
        self.declare(Fact(name=name__, repr=repr__))

    @Rule(
        OR(
            InterestPlace("Северная петля-Рабочий и колхозница"),
            InterestPlace("Монреаль"),
            InterestPlace("Павильон 75"),
            InterestPlace("Павильон 48")
        )
    )
    def fair_route(self):
        name__ = 'Ярморочный маршрут "Выставка вне времени"'
        repr__ = """На нашей экскурсии вы узнаете, как развивалась выставочная деятельность с царских времён, о том, как ярмарки повлияли на современные выставки.
        Кроме того, во время нашего путешествия вы познакомитесь с тем, как Россия дебютировала на международной арене, а также узнаете, что на протяжении многих лет именно российские павильоны были фаворитами на ЭКСПО."""
        self.declare(Fact(name=name__, repr=repr__))

    @Rule(
        OR(
            InterestPlace("Десятилетие науки и технологий"),
            InterestPlace("Павильон Космос"),
            InterestPlace("Дом полимеров"),
            InterestPlace("Мир цифры"),
            InterestPlace("Сделано нами (Минпромторг)")
        )
    )
    def scientific_route(self):
        name__ = 'Наука - Это просто!'
        repr__ = """Участники экскурсии убедятся в том, что наука может быть увлекательной. Они познакомятся с современными научными достижениями и разработками в области энергетики, экологии, медицины, сельского хозяйства, узнают о действующих образовательных программах для построения карьеры в науке. Экскурсоводы расскажут яркие истории успеха ученых прошлого и настоящего.
        Гости выставки попробуют себя в роли естествоиспытателей и совершат несколько несложных опытов. Экскурсия разработана совместно с Министерством науки и высшего образования Российской Федерации и приурочена к Десятилетию науки и технологий в России."""
        self.declare(Fact(name=name__, repr=repr__))

    @Rule(
        Fact("Будни"),
        OR(
            InterestPlace("Площадь промышленности"),
            InterestPlace("Фонтан \"Каменный цветок\""),
            InterestPlace("Выводные круги")
        )
    )
    def immersive_route(self):
        name__ = 'Иммерсивный маршрут "Путешествие в будущее!"'
        repr__ = """Иммерсивный маршрут позволит участникам окунуться в атмосферу фильмов о завтрашнем дне. Экскурсовод выступит студентом факультета космических исследований МГУ им. М.В. Ломоносова и на экспозициях выставки расскажет, каким он видит будущее. Проходя по локациям маршрута, гости познакомятся с необычной девушкой, которая окажется гостьей из будущего…"""
        self.declare(Fact(name=name__, repr=repr__))

    @DefFacts()
    def get_facts(self, fact_list: dict):
        yield CountVisitors(fact_list['cnt_visitors'])
        yield ChildrenAge(age=fact_list['minimal_age'])

        for place in fact_list['places']:
            yield InterestPlace(name=place)


def survey():

    def get_places():
        list_of_places = ("Площадь промышленности", "Павильон №1 (Движение первых)",
                          "Фонтан \"Дружба Народов\"", "Фонтан \"Каменный Цветок\"",
                          "Техноград", "Павильон Атом", "Москвариум", "Павильон Космос",
                          "Сочинская горка", "Музей кино", "Запрудная зона", "Аллея славы",
                          "Дом молодёжи", "Галерея достижений", "Монреаль", "Павильон 75",
                          "Павильон 48", "Сделано нами (Минпромторг)", "Мир цифры",
                          "Дом полимеров"
                          )

        places_str = "\n".join(f"{idx + 1}) {place}" for idx,
                               place in enumerate(list_of_places))
        places_idx = map(int, input(
            "Список павильонов, по которым проходят маршруты:" + f"\n{places_str}\n" +
            "Укажите павильоны, которые хотели бы посетить: ").split())

        result = list(list_of_places[idx - 1] for idx in places_idx)

        return result

    def get_cnt_visitors():
        cnt_visitors = int(input("Укажите пожалуйста, сколько вас человек: "))
        return cnt_visitors

    def get_weekday():
        days = ["Вторник", "Среда", "Четверг",
                "Пятница", "Суббота", "Воскресенье"]
        days_enum = "\n".join(f"{idx + 1}) {weekday}" for idx,
                              weekday in enumerate(days))

        weekdays_input = map(int, input(
            "\t!Важная информация!\n" +
            "\tВ понедельник выставка и экскурсии не работают.\n" +
            f"Укажите, в какой день недели, вы хотите посетить экскурсию (можно несколько): {days_enum}").split())
        result = list(days[idx - 1] for idx in weekdays_input)

        return result

    def get_info_about_children():
        children_status = input(
        "Укажите, есть ли в вашей группе дети (Да / Нет): ")

        if children_status.lower() not in ("да", "нет", "yes", "no", "y", "n"):
            raise ValueError("Недопустимый ответ")
        
        
        min_children = 18

        if children_status == "Да":
            min_children = int(input("Укажите минимальный возраст детей: "))

        return min_children

    result = dict()
    print("""Добро пожаловать на выставку \"Россия\".
          Я бот, который поможет Вам выбрать экскурсию для посещения, давайте пообщаемся..""")

    cnt_visitors = get_cnt_visitors()
    min_children = get_info_about_children()
    weekday = get_weekday()
    places = get_places()

    result['cnt_visitors'] = cnt_visitors
    result['minimal_age'] = min_children
    result['places'] = places
    result['weekday'] = weekday

    return result


def main():
    facts = survey()

    exhibit = Exhibition()
    exhibit.reset(fact_list=facts)
    exhibit.run()

    print(exhibit.facts)


if __name__ == '__main__':
    main()
