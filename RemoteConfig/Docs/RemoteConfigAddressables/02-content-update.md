# Пример 2. Новый скин через обновление каталога

**Задача:** запустить сцену с Ice и Lava, затем опубликовать Emerald и загрузить его по ключу Firebase.

Сцена: **Assets/Scenes/RemoteConfigAddressables/AddressablesContentUpdate.unity**.

В первом примере режим Use Asset Database видит все ассеты сразу. Здесь используем **Use Existing Build**, чтобы клиент сначала действительно не знал нового адреса.

Приложение собирать не нужно. Собираются только файлы Addressables. Для публикации во время работы сцены понадобятся две копии Unity-проекта и локальный HTTP-сервер.

## 1. Подготовьте исходный контент

Используйте свой файл Firebase, как в [первом примере](README.md#3-подключите-свой-firebase-проект). В Remote Config установите и опубликуйте:

```text
cube_visual_key = visuals/ice
```

В Addressables Groups группа **Remote Cube Visuals** должна содержать только **visuals/ice** и **visuals/lava**.

Третий prefab подготовлен в **Assets/Examples/RemoteConfigAddressables/NextContent/Emerald.prefab**. Пока не добавляйте его в Addressables и не перетаскивайте в сцену.

В настройках Addressables уже включены:
- **Build Remote Catalog** — публикуется каталог с адресами.
- **Disable Catalog Update on Startup** — обновлением управляет код примера.
- **Unique Bundle IDs** — позволяет обновлять контент при загруженном предыдущем скине.

Профиль использует Remote.BuildPath = **ServerData/[BuildTarget]** и Remote.LoadPath = **http://127.0.0.1:8000/[BuildTarget]**. Оставьте одну и ту же платформу и профиль на всех шагах.

## 2. Соберите исходную версию

Выйдите из Play Mode.

В **Window → Asset Management → Addressables → Groups** выберите **Build → New Build → Default Build Script**.

Сохраните копию созданного **addressables_content_state.bin** в отдельной папке: он описывает именно эту исходную сборку. В настройках Addressables путь указан в **Content State Build Path**; при стандартных настройках файл находится в подпапке платформы в Assets/AddressableAssetsData.

В папке **ServerData** появятся каталог, hash и bundles.

## 3. Подготовьте копию для публикации

Основной проект далее называем **клиентом**. Его сцену оставим работающей.

В отдельной папке создайте копию проекта для **публикации**: скопируйте из клиента папки **Assets**, **Packages** и **ProjectSettings**, обязательно вместе с файлами **.meta**. Library и Temp копировать не нужно.

Откройте эту копию через Unity Hub в той же версии Unity. Исходный addressables_content_state.bin из шага 2 понадобится при сборке обновления. Не создавайте новую исходную сборку в копии для публикации.

Оба редактора должны использовать одинаковую активную платформу.

## 4. Запустите клиент

В PowerShell перейдите в папку основного Unity-проекта, где лежит **ServerData**, и запустите:

```powershell
py -m http.server 8000 --bind 127.0.0.1 --directory ServerData
```

Команда требует Python 3 с Python Launcher. Оставьте окно открытым. Сервер можно остановить после занятия через Ctrl+C.

В клиенте выберите **Play Mode Script → Use Existing Build** и запустите сцену **AddressablesContentUpdate**. Должен появиться Ice.

**Не останавливайте Play Mode до конца демонстрации.**

## 5. Опубликуйте новый скин

Дальнейшие изменения ассетов выполняйте в **копии для публикации**.

1. Перетащите **NextContent/Emerald.prefab** в группу **Remote Cube Visuals**.
2. Установите ему адрес **visuals/emerald**.
3. Выберите **Build → Update a Previous Build**.
4. В открывшемся окне укажите сохранённый **addressables_content_state.bin исходной версии**.
5. После завершения скопируйте новые файлы из ServerData копии для публикации в ServerData клиента с сохранением подпапок платформы. Сначала перенесите bundles, затем каталог и в последнюю очередь hash. Файлы с совпадающими именами замените, старые bundles сохраните.

В результате HTTP-сервер клиента раздаёт обновлённый каталог и новый bundle. Код и исходные ассеты работающего клиента не менялись.

## 6. Передайте новый ключ через Firebase

Теперь измените **cube_visual_key** на **visuals/emerald** и опубликуйте параметр.

Во всё ещё работающей сцене клиента нажмите **Update from Firebase**.

Последовательность внутри игры:

```text
Получить cube_visual_key из Firebase
    → CheckForCatalogUpdates
    → UpdateCatalogs, если есть изменения
    → InstantiateAsync по новому ключу
```

Ожидаемый результат — зелёный скин и строка **Visual: visuals/emerald**.

Затем проверьте возврат к **visuals/lava** и **local/default** через тот же параметр. При ошибке загрузки предыдущий скин остаётся видимым.

## Что добавлено в код

Все скрипты находятся в **Assets/Scripts/RemoteConfigAddressables**.

Новый **AddressableCatalogUpdater.cs** проверяет и обновляет каталог, затем освобождает handles этих операций.

В **RemoteConfigAppearance** появилась ссылка **Catalog Updater**. Она заполнена только во второй сцене. Первый пример продолжает работать без обновления каталога. Значение local/default возвращает локальный куб без обращения к каталогу.

Нового кода поведения скина в обновлении нет: публикуется prefab с материалом, использующий существующие возможности приложения.

## Если не получилось

| Симптом | Проверка |
| --- | --- |
| Новый скин виден ещё до обновления | Клиент должен использовать Use Existing Build; Emerald не должен входить в исходный каталог. |
| No Location found for Key=visuals/emerald | Обновление успешно собрано из исходного content state; опубликованы новый каталог и hash; адрес совпадает. |
| Ошибка HTTP | Сервер запущен в папке клиента, в ServerData есть файлы нужной платформы. |
| Insecure connection not allowed | Player Settings → Other Settings → Allow downloads over HTTP = Development Only. |
| Каталог не обновляется | Копируются файлы по тому же адресу, который использовался в исходной сборке; hash тоже заменён. |
| Ошибка Firebase | Проверьте свой google-services.json, интернет и сообщение Console. |

Это проверка обновления контента в работающем Editor. Она не заменяет отдельную проверку на установленном приложении, если впоследствии понадобится такой показ.

