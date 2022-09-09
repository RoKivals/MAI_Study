#/bin/bash
#Переменная отвечающая за глубину поиска.
Depth=""
Count=0
#Функция, которая выводит пути всех подкаталогов в нужной нам директории.
#Используем так же значение по умолчанию, если не было передано никакого аргумента. В качестве значения по умолчанию используется каталог, из которого происходит вызов.
function check {
IFS=$'\n'
for file in `find ${1-"."} $Depth -type d | xargs -0`
do
    
 realpath $file;
done 
}

function keys {
while [[ -n "$1" ]];
do
case $1 in
    -h) 
        echo "This utility outputs the full path of all subdirectories of the specified directory."
        echo "It is also capable of handling an indefinite number of passed arguments."
        echo "The utility was written by Vyacheslav Orussky."
        exit;;

    -d) Depth="-maxdepth "$2
        shift
        shift
        Count=$((Count + 2))
        break;;

     *) 
        break;;
    esac
done
}


if [ -n "$1" ]
then
keys $@
for ((i=0; i<Count; i++))
    do
        shift
    done
    if [ -n "$1" ]
        then
            for x in $@
            do
              check $x
            done
    else
        check $1
    fi
else
    check $1
fi

