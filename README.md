# AdvancedC_RPG
 RPG game for advanced C# lessons #5/6

## Описание работы спавнера врагов:

Базовый скрипт спавнера находится в папке Assets/Developers Folder/Scripts/Spawner, а спавнер врагов находится в папке Assets/Developers Folder/Scripts/EnemySpawner



При инициализации в SceneBootstrapper, EnemySpawner получает массив Transform. Если Transform[] не пуст, то добавляет в свой список точек для спавна. Далее он берёт из конфига, который состоит из префаба врага, список оружия для него, вероятность его спавна и место куда будет добавляться оружие, устанавливает либо в AddMeleeEnemy либо AddRangedEnemy. Заспавненный противник заноситься в список заспавнившихся противников.

Оружия и префаб противника находит из папки в проекте.

Далее создаёт готовый объект на сцену.


## Описание работы спавнера босса:

BossSpawner (Assets/Developers Folder/Scripts/Entities/Enemy/Boss/BossSpawner.cs), вызывается из SceneBootstrapper и используюет фабрики магических снарядов (Assets/Developers Folder/Scripts/Magic/MagicProjectileFactory.cs) и катаны (Assets/Developers Folder/Scripts/Weapons/Melee/Katana/KatanaFactory.cs) для получения одного из четырех вариантов оружия.

У фабрик магии/катаны есть общая абстрактная реализация и 4 варианта спавна, переопределяющие материал и звуки/цвет эффектов. Элементы подгружаются через Adressables.

## Канал оповещения об убитых врагах

Спавн босса привязан к каналу DeathCount из статичного класса EventHub (Assets/Developers Folder/Scripts/EventChanel/EventHub.cs), собирающем информацию о каждой смерти врага и транслирующий текущее число убитых в канал. 
BossSpawner подписывается на канал через SceneBootstrapper и спавнит босса когда счетчик доходит до 3.

К этому же каналу привязано проигрывание звуков после убийства 5 врагов и обновление scoreboard (Assets/Developers Folder/Scripts/HUD/ScoreDisplay.cs).

