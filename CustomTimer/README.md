
Timer that calls the function after a specified time.

Use the following:

```
string message = "Timer expired";

Timer.Instance.AddTimerTask(5000, (arg) => {
    Console.WriteLine($"{message} - {DateTime.Now}");
}, message);
```