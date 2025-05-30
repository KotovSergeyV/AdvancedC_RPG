# AdvancedC_RPG
 RPG game for advanced C# lessons #5

## Описание работы спавнера:

Базовый скрипт спавнера находится в папке Assets/Developers Folder/Scripts/Spawner, а спавнер врагов находится в папке Assets/Developers Folder/Scripts/EnemySpawner



При инициализации в SceneBootstrapper, EnemySpawner получает массив Transform. Если Transform[] не пуст, то добавляет в свой список точек для спавна. Далее он берёт из конфига, который состоит из префаба врага, список оружия для него, вероятность его спавна и место куда будет добавляться оружие, устанавливает либо в AddMeleeEnemy либо AddRangedEnemy.

Оружия и префаб противника находит из папки в проекте.

Далее создаёт готовый объект на сцену.