EasyConstructor is a generator of constructor for C#

# How to Use

1. add `partial` keyword
2. set attributes

```c#
[EmptyConstructor(ConstructorAccessibility.Internal)]
[AllArgsConstructor]
[RequiredArgsConstructor]
public partial class Sample{
    public string str;
    public int num = 1;
}

public class App{
    public static void Main(string[] args){
        var emptyCtor = new Sample();
        var allArgsCtor = new Sample("str", 0);
        var requiredArgsCtor = new Sample("str");
    }
}
```
