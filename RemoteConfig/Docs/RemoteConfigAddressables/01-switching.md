# Пример 1: выбор prefab через Remote Config

Откройте сцену Assets/Scenes/RemoteConfigAddressables/AddressablesSwitching.unity.

В Firebase Remote Config нужен один строковый параметр:

| cube_visual_key | Что увидим |
| --- | --- |
| local/default | Локальный куб |
| visuals/ice | Ледяной prefab |
| visuals/lava | Лавовый prefab |

Опубликуйте значение и нажмите **Update from Firebase** в игре. При запуске обновление выполняется автоматически. Повторное нажатие заблокировано до окончания загрузки.

Скорость вращения постоянная. Скорость и масштаб из Firebase демонстрируются в старом примере.

## Загрузка по сети

1. Откройте Window > Asset Management > Addressables > Groups.
2. Соберите контент: Build > New Build > Default Build Script.
3. Из папки Unity-проекта RemoteConfig запустите:
   `py -m http.server 8000 --bind 127.0.0.1 --directory ServerData`
4. В Groups выберите Play Mode Script > Use Existing Build и нажмите Play.

Use Asset Database позволяет быстро посмотреть сцену, но не проверяет HTTP-загрузку.

RemoteLoadPath уже настроен на http://127.0.0.1:8000/[BuildTarget]. Для проверки вне Editor собирайте Development Build с этой сценой первой: локальный HTTP разрешён только для development. Для телефона нужен доступный HTTPS-хостинг: замените RemoteLoadPath, пересоберите и опубликуйте контент.

## Что происходит в коде

Remote Config получает cube_visual_key. Addressables.InstantiateAsync создаёт выбранный prefab. Только после успешной загрузки старый экземпляр освобождается через ReleaseInstance. Значение local/default возвращает встроенный куб.

При ошибке сети или адреса старый облик остаётся на месте; подробности есть в Console. Кнопка позволяет повторить попытку, включая неудачную инициализацию Firebase. Незавершённая загрузка освобождается после завершения, если сцена уже закрыта.

Нулевой интервал FetchAsync нужен для ручной демонстрации, а не для частого опроса в готовой игре.

