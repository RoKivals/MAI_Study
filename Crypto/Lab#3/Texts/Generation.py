from nltk.corpus import wordnet
import random
import string


def random_words_text(filename, cnt: int):
    # Получение рандомных синонимов
    random_synsets = random.sample(list(wordnet.all_synsets()), cnt)
    # Извлечение всех синонимов в лексической форме
    random_words = [synset.lemma_names() for synset in random_synsets]

    # Очищаем список от слов с символами подчёркивания
    words = list()
    for i in random_words:
        if '_' in i or '-' in i:
            continue
        words.extend(i)

    file = open(f'Lab#3/Texts/{filename}', 'w')
    file.write(' '.join(words))
    file.close()


def random_letters_text(cnt: int):
    res = ''
    for _ in range(cnt):
        res = res + random.choice(string.ascii_letters)

    return res


def match_percentage(text1, text2, length):
    matched = 0
    for i in range(length):
        if text1[i] == text2[i]:
            matched += 1

    return matched / length


def preprocess(lines):
    return ' '.join(lines).lower()


random_words_text("random_words_1.txt", 1000)
random_words_text("random_words_2.txt", 1000)
