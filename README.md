# SnifferTestThird
Задача: Язык С++ или C#. Среда разработки не ниже Visual Studio 2019.
 
Задача. Написать консольное приложение sniffer.exe под Windows. Приложение прослушивает трафик проходящий через Ethernet порт на уровне протокола IP.
sniffer.exe
Первый параметр - IP связанный с ethernet портом
Второй параметр - файл для записи данных о прошедшем трафике через порт.
пример:
sniffer.exe 192.168.1.200 data.log
 
При старте настроить сырой сокет на прослушивание входящего/исходящего трафика. В файл сохранять только распарсенные заголовки каждого IP пакета. В случае если IP пакет содержит UDP или TCP пакет - распарсить и их заголовок тоже и сохранить.
Формат записи в файл - на ваше усмотрение, но записи должны читаться из простейших текстовых редакторов типа блокнота.

-------------------------------------------------------------------------------
От себя:
Нашел готовый проект MSJSniffer и разбирался, как все работает. То есть написал не с 0, а скорее переписывал код и смотрел, как это устроено.
Разобрался.  
