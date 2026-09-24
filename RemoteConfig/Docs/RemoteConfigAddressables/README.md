# Пример 1. Переключение скинов через Firebase Remote Config

**Результат:** вы меняете ключ в своём Firebase-проекте, нажимаете кнопку в игре и видите другой скин куба.

Пример запускается в **Unity Editor**. Firebase передаёт ключ, а Addressables по этому ключу загружает prefab из ассетов проекта. Собирать приложение или Addressables-контент, запускать сервер и устанавливать Python не требуется.

## 1. Откройте проект и сцену

В Unity Hub откройте папку **RemoteConfig** внутри репозитория firebase-examples. В ней находятся Assets, Packages и ProjectSettings.

Версия проекта — **Unity 6000.3.5f2**. Дождитесь окончания импорта и компиляции.

В окне **Project** откройте двойным щелчком:

```text
Assets/Scenes/RemoteConfigAddressables/AddressablesSwitching.unity
```

## 2. Проверьте скины в Addressables

Откройте **Window → Asset Management → Addressables → Groups**.

Разверните группу **Remote Cube Visuals**:

| Prefab | Address — ключ загрузки |
| --- | --- |
| Ice | visuals/ice |
| Lava | visuals/lava |

Сами prefab находятся в **Assets/Examples/RemoteConfigAddressables/Prefabs**.

В верхней части окна Groups выберите:

**Play Mode Script → Use Asset Database (fastest)**

В этом режиме Addressables находит ассеты непосредственно в Unity-проекте. Обращения к серверу с файлами скинов не будет.

Если меню Addressables отсутствует, дождитесь импорта и проверьте ошибки в Console. Пакет уже добавлен в проект.

## 3. Подключите свой Firebase-проект

Выполняйте настройку при выключенном Play Mode.

1. Откройте свой проект в Firebase Console.
2. В настройках проекта выберите своё Android-приложение и скачайте **google-services.json**. Если приложение ещё не зарегистрировано, зарегистрируйте его с Android Package Name из Unity: **Edit → Project Settings → Player → Android → Other Settings → Identification → Package Name**. Собирать Android-приложение для этого не нужно.
3. Поместите **свой файл** в **Assets/google-services.json**, заменив файл из примера. Имя должно быть ровно google-services.json, без суффиксов вроде (1).
4. Дождитесь обработки файла в Unity. Firebase SDK автоматически создаёт конфигурацию для Editor — **google-services-desktop.json**. Самостоятельно заполнять её не нужно. [Документация Firebase](https://firebase.google.com/docs/unity/setup#desktop-workflow)
5. В том же своём Firebase-проекте откройте раздел **Remote Config** и добавьте параметр:

| Поле | Значение |
| --- | --- |
| Имя параметра | cube_visual_key |
| Тип | String |
| Значение по умолчанию | visuals/ice |

Сохраните и **опубликуйте изменения**. Для первого запуска используйте значение по умолчанию без дополнительных условий. Кавычки в значение добавлять не нужно.

## 4. Запустите сцену

Нажмите **Play** в Unity и откройте вкладку **Game**.

При запуске сцена сама получает параметр из Firebase. Ожидаемый результат:

- Куб вращается и получает ледяной скин.
- Строка состояния показывает **Visual: visuals/ice**.
- Под ней находится кнопка **Update from Firebase**.

Во время запроса отображается **Loading...**, а кнопка временно недоступна. Для получения параметров Firebase нужен интернет.

## 5. Переключите скин

Оставьте сцену запущенной.

В Firebase измените **cube_visual_key** на **visuals/lava** и **опубликуйте** изменения. Вернитесь во вкладку Game и нажмите **Update from Firebase**.

Куб должен получить лавовый скин. Попробуйте все значения:

| Значение | Результат |
| --- | --- |
| visuals/ice | Ледяной скин |
| visuals/lava | Лавовый скин |
| local/default | Обычный куб |

После каждого изменения нужны **публикация в Firebase** и **нажатие кнопки в игре**. Автоматической подписки на изменения в этом примере нет.

## Если не получилось

| Симптом | Что проверить |
| --- | --- |
| Нет кнопки | Открыта AddressablesSwitching, включён Play Mode и выбрана вкладка Game. |
| Требуется сборка или появляется ошибка HTTP | В Addressables Groups выберите Play Mode Script → Use Asset Database (fastest), затем заново запустите Play Mode. |
| No Location found for Key | Ключ написан точно как адрес в группе: visuals/ice или visuals/lava. |
| Остался обычный куб | Параметр опубликован в вашем Firebase-проекте. Значение local/default специально показывает обычный куб. |
| Ошибка Firebase или Could not update | Проверьте интернет и сообщение в Window → General → Console. Убедитесь, что в Assets лежит ваш google-services.json. После исправления нажмите кнопку ещё раз. |
| Unity продолжает использовать старый Firebase-проект | Остановите Play Mode. Сравните project_info.project_id в Assets/google-services.json и Assets/StreamingAssets/google-services-desktop.json. Если в созданном SDK файле остался старый проект, удалите его через окно Project, выполните Reimport для своего google-services.json и перезапустите Unity. |

## Где посмотреть передачу ключа

Скрипты находятся в **Assets/Scripts/RemoteConfigAddressables**:

1. **RemoteConfigAppearance.cs** читает cube_visual_key из Firebase и передаёт строку в ShowAsync.
2. **AddressableCubeView.cs** вызывает Addressables.InstantiateAsync по этому ключу и освобождает предыдущий скин через ReleaseInstance.
3. **AppearanceDemoPanel.cs** показывает состояние и кнопку обновления.

Готово, когда вы можете переключить **Ice → Lava → обычный куб** через свой Firebase.

