# The DBContext

the *DBContext* class exists for the sole purpose of facilitating the following functionality:

- [Automatic audit fields](DB-Instances-Audit-Fields.md)
- [Custom event hooks](DB-Instances-Event-Hooks.md)
- [Global filters](DB-Instances-Global-Filters.md)
- [Dependancy injection](DB-Instances.md#dependancy-injection) (debatable)

## Create an instance

```csharp
var db = new DBContext("database-name", "127.0.0.1");
```
you only need to supply the connection parameters to the constructor if you haven't initialized the same database connection before in your application. 
if for example you have done: `await DB.InitAsync(...)` in your app startup, then you can simply do `new DBContext()` without supplying any parameters.

**Note:**
the DBContext constructor does **NOT** try to establish network connectivity with the server immediately. it would only establish connection during the very first operation perfomed by the DBContext instance. whereas the `DB.InitAsync()` method would establish connectivity immediately and throw an exception if unsuccessful.

## Perform operations

all operations supported by the static `DB` class are available via DBContext instances like so:
```csharp
await db.SaveAsync(new Book { Title = "test" });

await db.Find<Book>()
        .Match(b => b.Title == "test")
        .ExecuteAsync();

await db.Update<Book>()
        .MatchID("xxxxxxxxxx")
        .Modify(b => b.Title, "updated")
        .ExecuteAsync();
```

## Dependancy injection

it may be tempting to register `DBContext` instances with IOC containers. instead you should be injecting the repositories (that wrap up data access methods) into your controllers/services, not the DBContext instances directly. [click here](https://github.com/dj-nitehawk/MongoDB-Entities-Repository-Pattern) for a repository pattern example.

if you don't plan to unit test or swap persistance technology at a future date, there's really no need to use dependency injection and/or DBcontext instances *(unless you need the features mentioned above)*, you are then free to do everything via the DB static methods for the sake of convenience.

it is however recommended that you encapsulate all data access logic in repository/service/manager classes in order to isolate persistance logic from your application logic.

> [!tip]
> as an alternative, have a look at **vertical slice architecture** as done [_here_](https://github.com/dj-nitehawk/MongoWebApiStarter) for a far superior developer experience compared to the commonly used layerd+di+repositories mess.