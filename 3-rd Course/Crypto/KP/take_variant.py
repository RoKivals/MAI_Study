from pygost.gost34112012 import GOST34112012

name = "Пeтрoв Илья Олегoвич"

text = name.encode('utf-8')
hash_object = GOST34112012(digest_size=32)
hash_object.update(text)

hash_hex = hash_object.hexdigest()
print(hash_hex)