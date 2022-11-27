# Speak

Сервис Web-RTC видео-конференций на .NET 6 и Angular 14 

Посмотреть на его работу можно вот тут: https://face-2-face.ru

## Клиент

Для его запуска потребуется сделать сертификаты, например при помощи mkcert

Путь до сертификатов указывается в параметрах при запуске вот так:
 
`ng serve -o --host 0.0.0.0 --ssl true --ssl-key ../.cert/key.pem  --ssl-cert ../.cert/cert.pem`

## Бэк

Бэк на шестых корях

Заюзан SignalR для веб-сокет соединения с бэком и обмена "сигналами" между клиентами

Чтоб все работало, понадобятся те же самые сертификаты, что и фронту. Кинуть их нужно в корень, либо указать путь в appsettings.Development

## Как это работает?

В созвоне может участвовать неограниченное количество человек

Каждый зашедший нажимает кнопку "Подключиться" и все уже участвующие в созвоне клиенты получают сообщение о новом пользователе, а получив - отсылают ему оффер

Новый участник получает офферы, отсылает ансверы в ответ, затем начинается обмен ICE-кандидатами

После окончания обмена все участники видят медиа-потоки друг друга