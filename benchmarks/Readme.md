BenchmarkDotNet=v0.13.2, OS=ubuntu 20.04
Intel Xeon CPU E5-2686 v4 2.30GHz, 1 CPU, 2 logical and 2 physical cores
.NET SDK=6.0.403
  [Host]     : .NET 6.0.11 (6.0.1122.52304), X64 RyuJIT AVX2 DEBUG
  DefaultJob : .NET 6.0.11 (6.0.1122.52304), X64 RyuJIT AVX2


|                    Method |     Mean |    Error |   StdDev |    Gen0 |   Gen1 |  Allocated |
|-------------------------- |---------:|---------:|---------:|--------:|-------:|-----------:|
| NewtonsoftDeserialization | 525.2 us | 10.40 us | 12.38 us |  3.9063 |      - |  106.28 KB |
|   FsharpLuDeserialization | 500.4 us |  4.19 us |  3.71 us |  6.8359 | 0.9766 |  162.78 KB |
| SystemTextDeserialization | 523.4 us |  5.68 us |  5.04 us |  0.9766 |      - |   30.36 KB |
|   ThothNetDeserialization | 462.1 us |  5.37 us |  5.28 us |  9.7656 | 3.4180 |  232.95 KB |
|      JeanDeserialization | 229.3 us |  3.13 us |  2.93 us |  6.1035 | 1.4648 |  141.41 KB |
|       BareDeserialization | 602.9 us | 11.97 us | 24.44 us | 55.6641 | 6.8359 | 1291.71 KB |


|                  Method |      Mean |    Error |   StdDev |   Gen0 |   Gen1 | Allocated |
|------------------------ |----------:|---------:|---------:|-------:|-------:|----------:|
| NewtonsoftSerialization | 237.89 us | 1.340 us | 1.254 us | 2.9297 |      - |  73.32 KB |
|   FsharpLuSerialization | 230.00 us | 1.972 us | 1.647 us | 4.3945 | 0.4883 | 103.85 KB |
| SystemTextSerialization |  72.25 us | 1.037 us | 0.970 us | 1.0986 |      - |  26.88 KB |
|   ThothNetSerialization | 174.86 us | 1.029 us | 0.912 us | 6.8359 | 1.9531 |  157.8 KB |
|      JeanSerialization | 115.68 us | 0.276 us | 0.231 us | 3.7842 | 0.4883 |  88.77 KB |
|       BareSerialization | 103.18 us | 2.020 us | 2.960 us | 6.1035 | 0.1221 |  142.6 KB |

