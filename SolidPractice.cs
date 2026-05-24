using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

//Задание №1

public class NetworkPlayer { }
public class NetworkManager { }
public class NetworkObject { }

public interface INetworkListener
{
    void OnPlayerJoined(NetworkPlayer player);
    void OnPlayerLeft(NetworkPlayer player);
    void OnHostMigrationStarted(NetworkManager manager);
    void OnHostMigrationEnded(NetworkManager manager);
    void OnObjectSpawned(NetworkObject obj);
    void OnObjectDestroyed(NetworkObject obj);
}

//Нарушение Interface Segregation

//Решение:

public interface INetworkPlayerListener
{
    void OnPlayerJoined(NetworkPlayer player);
    void OnPlayerLeft(NetworkPlayer player);
}

public interface INetworkHostMigrationListener
{
    void OnHostMigrationStarted(NetworkManager manager);
    void OnHostMigrationEnded(NetworkManager manager);
}

public interface IObjectListener
{
    void OnObjectSpawned(NetworkObject obj);
    void OnObjectDestroyed(NetworkObject obj);
}


//Задание №2
public abstract class Character : MonoBehaviour
{
    public abstract void Move(Vector3 pos);

    public abstract Vector3 GetPosition();
}

public class Controller : MonoBehaviour
{
    [SerializeField]
    private Character _character;
    [SerializeField]
    private Camera _camera;

    private void Update()
    {
        float dx = Input.GetAxis("Horizontal");
        float dy = Input.GetAxis("Vertical");
        _character.Move(new Vector3(dx, 0, dy));
    }

    private void LateUpdate()
    {
        Vector3 position = _character.GetPosition() + new Vector3(0, 5, -5);
        _camera.transform.position = position;
    }
}

//Нарушение Single Responsibility. Одновременно идет управление камерой и движением персонажа.

//Решение

public class MoveController : MonoBehaviour
{
    [SerializeField]
    private Character _character;

    private void Update()
    {
        float dx = Input.GetAxis("Horizontal");
        float dy = Input.GetAxis("Vertical");
        _character.Move(new Vector3(dx, 0, dy));
    }
}

public class CameraFollowController : MonoBehaviour
{
    [SerializeField]
    private Character _character;
    [SerializeField]
    private Camera _camera;

    private void LateUpdate()
    {
        Vector3 position = _character.GetPosition() + new Vector3(0, 5, -5);
        _camera.transform.position = position;
    }
}

//Задание №3
public class AppsFlyer : MonoBehaviour 
{
    public void Initialize()
    {
        // что-то дклает
    }
}

public class LoadingScreen : MonoBehaviour
{
    public void Show()
    {
        // что-то дклает
    }

    public void Hide()
    {
        // что-то дклает
    }
}

public class GooglePlayServices 
{
    public async Task Authorize()
    {
        // что-то дклает
    }
}

public class GameRepository : MonoBehaviour
{
    public async Task LoadUserData() 
    {
        // что-то дклает
    }
}

public sealed class Bootstrap : MonoBehaviour
{
    [SerializeField] private AppsFlyer _appsFlyer;
    [SerializeField] private LoadingScreen _loadingScreen;
    [SerializeField] private GooglePlayServices _googlePlayServices;
    [SerializeField] private GameRepository _repository;
    public async void Initialize()
    {
        _appsFlyer.Initialize();
        _loadingScreen.Show();
        await _googlePlayServices.Authorize();
        await _repository.LoadUserData();
        await SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Additive);
        _loadingScreen.Hide();
    }
}

/*Нарушение Open-Closed, так как возможна ситуация появление новых АПИ,
которые нужно инициализировать. Нарушение Dependency Inversion,
должна быть зависимость от выполнения абстракций сервисов или АПИ.
*/

//Решение

public sealed class Bootstrap_2 : MonoBehaviour
{
    [SerializeField] private LoadingScreen _loadingScreen;

    public IEnumerable<ILoadingTask> _loadingTasks;

    public void Construct(IEnumerable<ILoadingTask> loadingTasks) 
    {
        _loadingTasks = loadingTasks;
    }

    public async void Initialize()
    {
        _loadingScreen.Show();

        foreach (var task in _loadingTasks) 
        {
            await task.Do();
        }

        _loadingScreen.Hide();
    }
}

public interface ILoadingTask
{
    Task Do();
}


[Serializable]
public class StartServicesTask : ILoadingTask
{
    [SerializeField] private GooglePlayServices _googlePlayServices;
    [SerializeField] private GameRepository _repository;

    public async Task Do()
    {
        await _googlePlayServices.Authorize();
        await _repository.LoadUserData();
        await SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Additive);
    }
}

[Serializable]
public class AppsFlyerTask : ILoadingTask 
{
    [SerializeField]
    private AppsFlyer _appsFlyer;

    public Task Do() 
    {
        _appsFlyer.Initialize();
        return Task.CompletedTask;
    }
}

//Задание №4

public class Weapon : MonoBehaviour
{
    [SerializeField]
    protected Transform _transform;

    [SerializeField]
    private GameObject _bulletPrefab;

    [SerializeField]
    private float _countdown;

    private float _time;

    public void Fire()
    {
        if (_time <= 0)
        {
            Instantiate(_bulletPrefab, _transform.position, _transform.rotation);
            _time = _countdown;
        }
    }

    protected virtual void Update()
    {
        _time -= Mathf.Max(0, _time, Time.deltaTime);
    }
}
public class Turret : Weapon
{
    [SerializeField]
    private float _fireDistance;

    protected override void Update()
    {
        base.Update();
        Ray ray = new Ray(_transform.position, _transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, _fireDistance) &&
       hit.collider.CompareTag("Character"))
            this.Fire();
    }
}

//Нарушение Liskov Substitution. Может нарушиться корректность работы программы из-за
//возможности переорпеделения Update метода у базового класса. Нужно использховать делегирование вместо наследования
//Решение
public interface IWeapon 
{
    public void Fire();
}

public abstract class WeaponBase: MonoBehaviour, IWeapon
{
    [SerializeField]
    protected Transform _transform;

    [SerializeField]
    private GameObject _bulletPrefab;

    [SerializeField]
    private float _countdown;

    private float _time;

    public void Fire() 
    {
        if (_time <= 0)
        {
            Instantiate(_bulletPrefab, _transform.position, _transform.rotation);
            _time = _countdown;
        }
    }

    private void Update()
    {
        _time -= Mathf.Max(0, _time - Time.deltaTime);
        Fire();
    }
}

public class Turret_1 : WeaponBase
{ 
    [SerializeField]
    private float _fireDistance;

    private IWeapon _weapon;

    public void SetWeapon(IWeapon weapon) => _weapon = weapon;

    private void Update()
    {
        Ray ray = new Ray(_transform.position, _transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, _fireDistance) &&
            hit.collider.CompareTag("Character")) 
        {
            _weapon.Fire();
        }
    }
}

//Задание №5
public class ChessBoard
{
    public ChessSnapshot GetSnapshot()
    {
        throw new NotImplementedException();
    }

    public void Restore(string item2)
    {
        throw new NotImplementedException();
    }
}
public class ServerStorage
{
    public async Task RequestSaveGame(ChessSnapshot boardData)
    {
        
    }

    public async Task<(bool, string)> RequestLoadGame(int version)
    {
        throw new NotImplementedException();
    }
}
public class ChessSnapshot 
{

}

public class ChessSaveService
{
    private readonly ChessBoard _board;
    private readonly ServerStorage _server;
    public ChessSaveService(ChessBoard board, ServerStorage server)
    {
        _board = board;
        _server = server;
    }
    public async Task Save()
    {
        ChessSnapshot boardData = _board.GetSnapshot();
        await _server.RequestSaveGame(boardData);
    }
    public async Task Load(int version)
    {
        (bool, string) result = await _server.RequestLoadGame(version);
        if (result.Item1)
            _board.Restore(result.Item2);
    }
}

//Нарушение Dependency Inversion. ChessSaveService должен зависеть от абстракций, а не классов ChessBoard и ServerStorage
// Решение

public interface IChessBoard 
{
    public ChessSnapshot GetSnapshot();

    public void Restore(string item2);  
}
public interface IServerStorage
{
    public Task RequestSaveGame(ChessSnapshot boardData);

    public Task<(bool, string)> RequestLoadGame(int version);  
}
public class ChessSaveService_1
{
    private readonly IChessBoard _board;
    private readonly IServerStorage _server;
    public ChessSaveService_1(IChessBoard board, IServerStorage server)
    {
        _board = board;
        _server = server;
    }
    public async Task Save()
    {
        ChessSnapshot boardData = _board.GetSnapshot();
        await _server.RequestSaveGame(boardData);
    }
    public async Task Load(int version)
    {
        (bool, string) result = await _server.RequestLoadGame(version);
        if (result.Item1)
            _board.Restore(result.Item2);
    }
}

//Задание №6
// 2 Балла
public class Item
{
    public void Apply(Character character)
    {
        throw new NotImplementedException();
    }

    public void Discard(Character character)
    {
        throw new NotImplementedException();
    }
}

public sealed class Inventory : MonoBehaviour
{
    public event Action OnItemAdded;
    public event Action OnItemRemoved;
    [SerializeField]
    private readonly Character _character;

    private readonly List<Item> items = new();
    public bool AddItem(Item item)
    {
        if (this.items.Contains(item))
            return false;

        this.items.Add(item);
        item.Apply(_character);

        this.OnItemAdded?.Invoke();
        return true;
    }
    public bool RemoveItem(Item item)
    {
        if (!this.items.Remove(item))
            return false;
        item.Discard(_character);
        this.OnItemRemoved?.Invoke();
        return true;
    }
}

//Нарушение SRP. Нужно применить Observer. Inventory одновременно занимается айтемами и персонажем, чего быть не должно 
//Решение:
public sealed class Inventory_1 : MonoBehaviour
{
    public Action<Item> OnItemAdded;
    public Action<Item> OnItemRemoved;
    [SerializeField]
    private readonly Character _character;

    private readonly List<Item> items = new();
    public bool AddItem(Item item)
    {
        if (this.items.Contains(item))
            return false;

        this.items.Add(item);

        this.OnItemAdded?.Invoke(item);
        return true;
    }
    public bool RemoveItem(Item item)
    {
        if (!this.items.Remove(item))
            return false;

        this.OnItemRemoved?.Invoke(item);
        return true;
    }
}

public class CharacterInventoryController : MonoBehaviour
{
    [SerializeField]
    private Inventory_1 _inventory;
    [SerializeField]
    private Character _character;
    private void OnEnable()
    {
        _inventory.OnItemAdded += OnItemAdded;
        _inventory.OnItemRemoved += this.OnItemRemoved;
    }
    private void OnDisable()
    {
        _inventory.OnItemAdded -= this.OnItemAdded;
        _inventory.OnItemRemoved -= this.OnItemRemoved;
    }
    private void OnItemAdded(Item item) => item.Apply(_character);
    private void OnItemRemoved(Item item) => item.Discard(_character);
}

//Задание №7
public class Medkit : MonoBehaviour
{
    [field: SerializeField]
    public int HealPoints { get; private set; }
}
public class Ammo : MonoBehaviour
{
    [field: SerializeField]
    public int Amount { get; private set; }
}
public class WeaponPickUp : MonoBehaviour
{
    [field: SerializeField]
    public Weapon WeaponPrefab { get; private set; }
}
public class Character_2 : MonoBehaviour
{
    [field: SerializeField]
    public int Health { get; private set; }

    [field: SerializeField]
    public int Ammo { get; private set; }

    [field: SerializeField]
    public Weapon CurrentWeapon { get; private set; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Medkit medkit))
        {
            this.Health += medkit.HealPoints;
            Destroy(medkit.gameObject);
        }
        else if (other.TryGetComponent(out Ammo ammo))
        {
            this.Ammo += ammo.Amount;
            Destroy(ammo.gameObject);
        }
        else if (other.TryGetComponent(out WeaponPickUp weapon))
        {
            if (this.CurrentWeapon != null)
                Destroy(this.CurrentWeapon.gameObject);

            this.CurrentWeapon = Instantiate(weapon.WeaponPrefab, this.transform);
            Destroy(weapon.gameObject);
        }
    }
}

//Нарушение Open-Closed. Появление новых классов потребует переписывание метода OnTriggerEnter
//Решение
public interface ICharacter_1 
{
    public void SetHealth(int healPoints);
    public void SetAmmo(int amount);
    public Weapon GetWeaponPrefab();
    public void SetWeapon(Weapon weapon);
}

public interface IPickUpItem
{
    public void PickUp(ICharacter_1 character);
}

public class Medkit_1 : MonoBehaviour, IPickUpItem
{
    [field: SerializeField]
    public int HealPoints { get; private set; }

    public void PickUp(ICharacter_1 character)
    {
        character.SetHealth(HealPoints);
    }
}
public class Ammo_1 : MonoBehaviour, IPickUpItem
{
    [field: SerializeField]
    public int Amount { get; private set; }

    public void PickUp(ICharacter_1 character)
    {
        character.SetAmmo(Amount);
    }
}
public class WeaponPickUp_1 : MonoBehaviour, IPickUpItem
{
    [field: SerializeField]
    public Weapon WeaponPrefab { get; private set; }

    public void PickUp(ICharacter_1 character)
    {
        var weapon = character.GetWeaponPrefab();

        if (weapon != null)
            Destroy(weapon.gameObject);

        character.SetWeapon(weapon);
    }
}

public class Character_3 : MonoBehaviour, ICharacter_1
{
    [field: SerializeField]
    public int Health { get; private set; }

    [field: SerializeField]
    public int Ammo { get; private set; }

    [field: SerializeField]
    public Weapon CurrentWeapon { get; private set; }

    public Weapon GetWeaponPrefab()
    {
        return CurrentWeapon;
    }

    public void SetAmmo(int amount)
    {
        this.Ammo += amount;
    }

    public void SetHealth(int healPoints)
    {
        this.Health += healPoints;
    }

    public void SetWeapon(Weapon weaponPrefab)
    {
        CurrentWeapon = Instantiate(weaponPrefab, this.transform);
        CurrentWeapon = weaponPrefab;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other is not IPickUpItem pickUpItem) return;

        pickUpItem.PickUp(this);
        Destroy(other.gameObject);
    }
}

//Задание №8
public class Bullet : MonoBehaviour{ }
public class Enemy : MonoBehaviour { }
public sealed class BulletPool
{
    private readonly Stack<Bullet> _bullets;
    private readonly Bullet _prefab;
    public BulletPool(Bullet prefab, int size)
    {
        _bullets = new Stack<Bullet>(size);
        for (var i = 0; i < size; i++)
            _bullets.Push(GameObject.Instantiate(prefab));
    }
    public Bullet Get()
    {
        if (_bullets.Count > 0)
            return _bullets.Pop();
        throw new Exception("Pool is empty");
    }
    public void Release(Bullet item)
    {
        _bullets.Push(item);
    }
}
public sealed class EnemyPool
{
    private readonly Queue<Enemy> enemies = new Queue<Enemy>();
    private readonly Enemy _prefab;
    public EnemyPool(Enemy prefab)
    {
        _prefab = prefab;
    }
    public Enemy Pull()
    {
        if (this.enemies.TryDequeue(out Enemy enemy))
            return enemy;
        return GameObject.Instantiate(_prefab);
    }
    public void Return(Enemy enemy)
    {
        this.enemies.Enqueue(enemy);
    }
}

//Применение Open-Closed с объединением классов

//Решение
public class ObjectPool<T> where T : MonoBehaviour
{
    private readonly Stack<T> _items = new();
    private readonly T _prefab;
    private readonly bool _fixedMode;
    public ObjectPool(T prefab, bool fixedMode, int initialSize)
    {
        _prefab = prefab;
        _fixedMode = fixedMode;
        for (int i = 0; i < initialSize; i++)
            _items.Push(GameObject.Instantiate(prefab));
    }
    public T Get() => _items.Count > 0
        ? _items.Pop()
        : _fixedMode
            ? throw new Exception("Pool is empty")
            : GameObject.Instantiate(_prefab);
    public void Release(T item)
    {
        _items.Push(item);
    }
}

//Задание №9
public class Reward
{
    public bool IsReady { get; internal set; }
    public object Money { get; internal set; }

    internal void Tick(float deltaTime)
    {
        throw new NotImplementedException();
    }
}

public class MoneyStorage
{
    internal void EarnMoney(object money)
    {
        throw new NotImplementedException();
    }
}
public class RewardConfig
{
    internal Reward InstantiateReward()
    {
        throw new NotImplementedException();
    }
}

public sealed class RewardFactory
{
    public Reward CreateReward(RewardConfig config)
    {
        return config.InstantiateReward();
    }
}
public sealed class RewardSelector
{
    private readonly RewardConfig[] catalog;
    public RewardSelector(RewardConfig[] catalog)
    {
        this.catalog = catalog;
    }
    public RewardConfig SelectReward()
    {
        int randomIndex = UnityEngine.Random.Range(0, this.catalog.Length);
        return this.catalog[randomIndex];
    }
}
public sealed class RewardService
{
    public Reward CurrentReward { get; set; }
}
public sealed class RewardGenerator
{
    private readonly RewardSelector selector;
    private readonly RewardFactory factory;
    private readonly RewardService service;
    public RewardGenerator(RewardSelector selector, RewardFactory factory, RewardService
   service)
    {
        this.selector = selector;
        this.factory = factory;
        this.service = service;
    }
    public void GenerateReward()
    {
        RewardConfig config = this.selector.SelectReward();
        Reward reward = this.factory.CreateReward(config);
        this.service.CurrentReward = reward;
    }
}
public sealed class RewardUpdater
{
    private readonly RewardService service;
    private readonly RewardReceiver receiver;
    public RewardUpdater(RewardService service, RewardReceiver receiver)
    {
        this.service = service;
        this.receiver = receiver;
    }
    public void Tick(float deltaTime)
    {
        Reward currentReward = this.service.CurrentReward;
        if (currentReward == null)
            return;
        currentReward.Tick(deltaTime);
        if (currentReward.IsReady)
            receiver.ReceiveReward();
    }
}
public sealed class RewardReceiver
{
    private readonly MoneyStorage moneyStorage;
    private readonly RewardService service;
    public RewardReceiver(MoneyStorage moneyStorage, RewardService service)
    {
        this.moneyStorage = moneyStorage;
        this.service = service;
    }
    public void ReceiveReward()
    {
        Reward reward = this.service.CurrentReward;
        this.moneyStorage.EarnMoney(reward.Money);
        this.service.CurrentReward = null;
    }
}

public interface IMoneyStorage
{
    void EarnMoney(object money);
}

// Объединение всего в один класс
public sealed class RewardManager
{
    public event Action<Reward> OnRewardGenerated;
    public event Action<Reward> OnRewardReceived;
    private readonly RewardConfig[] _catalog;
    private readonly IMoneyStorage _moneyStorage;
    private Reward _currentReward;
    public Reward CurrentReward => _currentReward;
    public RewardManager(RewardConfig[] catalog, IMoneyStorage moneyStorage)
    {
        _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
        _moneyStorage = moneyStorage ?? throw new ArgumentNullException(nameof(moneyStorage));
    }
    public void GenerateReward()
    {
        if (_catalog == null || _catalog.Length == 0)
            throw new InvalidOperationException("Catalog is empty");
        int randomIndex = UnityEngine.Random.Range(0, _catalog.Length);
        RewardConfig config = _catalog[randomIndex];
        _currentReward = config.InstantiateReward();
        OnRewardGenerated?.Invoke(_currentReward);
    }
    public void Tick(float deltaTime)
    {
        if (_currentReward == null) return;
        _currentReward.Tick(deltaTime);
        if (_currentReward.IsReady)
            ReceiveReward();
    }
    private void ReceiveReward()
    {
        if (_currentReward == null) return;
        _moneyStorage.EarnMoney(_currentReward.Money);
        OnRewardReceived?.Invoke(_currentReward);
        _currentReward = null;
    }
}
