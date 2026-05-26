class User:
    def __init__(self, login, type):
        self.login = login
        self._type = type

    @property
    def type(self):
        return self._type

    @type.setter
    def type(self, new_type: str):
        if new_type not in ['user', 'admin']:
            new_type = 'user'
        self._type = new_type
