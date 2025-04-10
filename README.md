﻿# EasyConstructor

EasyConstructorは自動でコンストラクタを自動生成するソースジェネレータを提供するライブラリです。

## Requirement

- .NET Standard 2.0+
- C#

## Install

[NuGetパッケージ](https://www.nuget.org/packages/EasyConstructor/)として配布されています。
コマンドを使用してインストールする場合は、以下のコマンドを使用してください。

```
dotnet add package EasyConstructor --version 0.2.0
```

## How To Use

[//]: # (SourceGenerator generate constructors of a class that has Attributes.)

コンストラクタを自動生成させたいクラスにAttributeを付けると、コンパイル時に自動生成されます。

この際、クラスに`partial`修飾子を付与してください。

例えば以下のように`[EmptyConstructor]`を付与すると、引数を持たない空のコンストラクタが自動生成されます。
```c#
namespace SampleNameSpace;
// You have to add the partial keyword. 
[EmptyConstructor]
public partial class EmptyClass {
    public EmptyClass(string arg){}
}

public class Sample {
    public static void Main(string[] args){
        var emptyInstance = new EmptyClass();
    }
}
```


`[AllArgsConstructor]`を付与すると、クラスに含まれる全てのフィールドに値を代入するコンストラクタが作成されます。
```c#
namespace SampleNameSpace;
// You have to add the partial keyword. 
[AllArgsConstructor]
public partial class EmptyClass {
    public string SampleStr;
}
```

`[RequiredArgsConstructor]`を付与すると、変数宣言時に初期化されないフィールドに値を代入するコンストラクタを作成します。
```c#
namespace SampleNameSpace;
// You have to add the partial keyword. 
[RequiredArgsConstructor]
public partial class EmptyClass {
    public string Str0 = "";
    public string Str1;
}
```

自動生成されるコンストラクタの対象となるのはフィールドのみです。**プロパティは対象にならないことに注意してください。**

また各Attributeの引数に`ConstructorAccessibility`型の値を渡すと、コンストラクタのアクセス修飾子を設定できます。

例えば以下のような場合、Str1を代入するProtectedキーワードが付いたコンストラクタが生成されます。

```c#
namespace SampleNameSpace;
// You have to add the partial keyword. 
[RequiredArgsConstructor(ConstructorAccessibility.Protected)]
public partial class RequiredClass {
    public string Str0 = "";
    public string Str1;
}
```

### Attributes

| `[EmptyConstructor]` | `[AllArgsConstructor]`          | `[RequiredArgsConstructor]`           |
|----------------------|---------------------------------|---------------------------------------|
| 引数を持たないコンストラクタを作成します | クラス内の全てのフィールドに代入するコンストラクタを作成します | クラス内の初期化されていないフィールドに代入するコンストラクタを作成します |

### ConstructorAccessibility

コンストラクタのアクセス修飾子は以下の6つから利用できます。

- `Public`
- `ProtectedInternal`
- `Protected`
- `Internal`
- `PrivateProtected`
- `Private`

## Roadmap

### 対応予定

- UPM化

### 検討中
