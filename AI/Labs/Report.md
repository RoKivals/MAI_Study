# Отчёт по лабораторной работе

Выполнили:

Студент (ФИО) | Роль в проекте   | Оценка
-------------|---------------------|------
Орусский Вячеслав Русланович | обучал Simple RNN с посимвольной и по-словной токенизацией | TBD
Петров Илья Олегович | Обучал однонаправленную однослойную и многослойную LSTM c посимвольной токенизацией и токенизацией по словам и на основе BPE | TBD
Карнаков Никита Дмитриевич | Отчет, обучал двунаправленную LSTM | TBD

В данном отчете будем приводить описание каждой из модели (процесс обучения, фрагменты кода и т.д.)

## LSTM с посимвольной токенизацией

# Предварительная обработка данных 

Из файла [data2.txt](data2.txt) извлекаются строки, игнорируются заголовки, такие как "Глава", и объединяются в один текстовый массив.

```python
raw = open('data2.txt', mode='r', encoding='utf-8').readlines()
data = []
for line in raw:
    if line != '\n' and 'Глава' not in line:
        data.append(' '.join(line.split()[1:]))
    
data = [line.replace('\n', ' ').replace('\xa0', ' ') for line in data]
text = ' '.join(data)
```

В коде строится алфавит всех уникальных символов, присутствующих в тексте, и каждому символу присваивается индекс. Токенизация происходит на уровне символов, что позволяет модели работать с текстом по букве.

```python
alphabet = np.array(sorted(set(text)))
word_index = {char: i for i, char in enumerate(alphabet)}
index_word = {i: char for i, char in enumerate(alphabet)}
```

# Создание обучающих данных

Текст превращается в последовательность индексов символов. Эти последовательности подаются в нейронную сеть с помощью пакетов (батчей).

```python
sequences = Dataset.from_tensor_slices(np.array([word_index[char] for char in text])).batch(BATCH_SIZE, drop_remainder=True)
dataset = sequences.map(get_features_target)

data = dataset.batch(BATCH_SIZE, drop_remainder=True).repeat()
data = data.prefetch(AUTOTUNE)
```
# Архитектура модели

Модель состоит из следующих слоёв:

* **Embedding слой**: Преобразует каждый символ в векторное представление;
* **LSTM слой**: Используется для обработки последовательностей и запоминания контекста;
* **Dense слой**: Выдаёт вероятности для каждого символа алфавита на выходе.

```python
model = keras.Sequential([
    keras.layers.Embedding(len(alphabet), 256),
    keras.layers.LSTM(512, return_sequences=True, stateful=True),
    keras.layers.Dense(len(alphabet))
])
```

# Компиляция и обучение модели

Модель компилируется с использованием стандартного оптимизатора `Adam` и функции потерь `SparseCategoricalCrossentropy`, подходящей для многоклассовой классификации.

```python
model.compile(optimizer='adam', loss=losses.SparseCategoricalCrossentropy(from_logits=True), metrics=['accuracy'])
model.fit(data, epochs=40, verbose=1, steps_per_epoch=len(sequences) // BATCH_SIZE)
```

# Генерация текста

Генерация текста происходит с помощью функции `predict_next`. Модель принимает начальную последовательность символов и предсказывает следующий символ, повторяя процесс до получения полной последовательности.

```python
def predict_next(sample, model, tokenizer, vocabulary, n_next, temperature, batch_size):
    sample_vector = [tokenizer[char] for char in sample]
    predicted = sample_vector
    sample_tensor = tf.expand_dims(sample_vector, 0)
    sample_tensor = tf.repeat(sample_tensor, batch_size, axis=0)
    
    for i in range(n_next):
        pred = model(sample_tensor)
        pred = pred[0].numpy() / temperature
        pred = tf.random.categorical(pred, num_samples=1)[-1, 0].numpy()
        predicted.append(pred)
        sample_tensor = predicted[-99:]
        sample_tensor = tf.expand_dims([pred], 0)
        sample_tensor = tf.repeat(sample_tensor, batch_size, axis=0)
    
    pred_seq = [vocabulary[i] for i in predicted]
    generated = ''.join(pred_seq)
    return generated
```

# Примеры предсказаний и анализ

**Пример 1 (с температурой 0.6)**

```python
print(predict_next(
    sample='б',
    model=model,
    tokenizer=word_index,
    vocabulary=index_word,
    n_next=200,
    temperature=0.6,
    batch_size=BATCH_SIZE
))
```
На выходе получаем следующее:

```plaintext
бом и селениями своими, ибо время согрешающего совершенного скота, о котором сказано: верен Бог их и умер, и освящающих нас, что делается всё, что значит им слово Божие, которое я имею тельцов и не дол
```

Этот пример показывает, что при температуре 0.6 текст получается разнообразным, но временами нарушается логика предложений. Модель способна генерировать осмысленные фразы, однако часть текста может не иметь связного смысла.

**Пример 2 (с температурой 0.2)**

```python
print(predict_next(
    sample='1',
    model=model,
    tokenizer=word_index,
    vocabulary=index_word,
    n_next=100,
    temperature=0.2,
    batch_size=BATCH_SIZE
))
```
На выходе получаем следующее:

```plaintext
1гая в сердцах своих и принятие в слове своём и сказали ему: вот, я возлюбленному своему и служению в
```

Здесь текст выглядит более предсказуемым, что объясняется низкой температурой (0.2). Однако начальный символ "1" явно выбивается из общего контекста. В то же время фразы более структурированы, хотя разнообразие предсказаний ниже.

# Выводы

Несмотря на работу с символами, модель способна генерировать текст, который напоминает исходные данные (религиозный текст). При этом, чем выше температура, появляются более разнообразные, но менее осмысленные фразы. При генерации длинных последовательностей иногда теряется связность между предложениями, что характерно для моделей, работающих на уровне символов.
