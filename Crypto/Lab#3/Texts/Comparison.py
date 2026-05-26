from Generation import match_percentage, preprocess, random_words_text, random_letters_text

with open('Lab#3/Texts/eng_text1.txt') as file1:
    norm_text1 = preprocess(file1.readlines())

with open('Lab#3/Texts/eng_text2.txt') as file2:
    norm_text2 = preprocess(file2.readlines())

with open('Lab#3/Texts/random_words_1.txt') as file1:
    rnd_words1 = preprocess(file1.readlines())

with open('Lab#3/Texts/random_words_2.txt') as file1:
    rnd_words2 = preprocess(file1.readlines())

rnd_text1 = random_letters_text(1000)
rnd_text2 = random_letters_text(1000)


def print_result(text1, text2):
    min_len = min(len(text1), len(text2))
    print(f'Индекс совпадения: {match_percentage(text1, text2, min_len)}; \
          количество символов: {min_len}')

print('1) Два осмысленных текста на естественном языке')
print_result(norm_text1, norm_text2)


print('2) Осмысленный текст и текст из случайных букв')
print_result(norm_text1, rnd_text1)


print('3) Осмысленный текст и текст из случайных слов')
print_result(norm_text1, rnd_words1)


print('4) два текста из случайных букв')
print_result(rnd_text1, rnd_text2)


print('5) два текста из случайных слов')
print_result(rnd_words1, rnd_words2)