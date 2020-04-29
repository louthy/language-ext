# Perfomance benchmarks

** Machine spec **

    BenchmarkDotNet=v0.11.5, OS=Windows 10.0.17134.1184 (1803/April2018Update/Redstone4)
    Intel Core i5-3470 CPU 3.20GHz (Ivy Bridge), 1 CPU, 4 logical and 4 physical cores
    Frequency=3117923 Hz, Resolution=320.7263 ns, Timer=TSC
    .NET Core SDK=3.1.100
      [Host]     : .NET Core 3.0.1 (CoreCLR 4.700.19.51502, CoreFX 4.700.19.51609), 64bit RyuJIT
      DefaultJob : .NET Core 3.0.1 (CoreCLR 4.700.19.51502, CoreFX 4.700.19.51609), 64bit RyuJIT
      
## Note

References to `Sys.Coll.Imm` are Microsoft's `ImmutableCollections` library.

* `Sys.Coll.Imm.List` is `System.Collections.Immutable.ImmutableList`
* `Sys.Coll.Imm.Dictionary` is `System.Collections.Immutable.ImmutableDictionary`
* `Sys.Coll.Imm.SortedDictionary` is `System.Collections.Immutable.ImmutableSortedDictionary`
* `Sys.Coll.Imm.SortedSet` is `System.Collections.Immutable.ImmutableSortedSet`
      

## Lists

### Lists with value types

Type | N | Add | RandomAccess | Iteration
----- | ----- | ----- | ----- | -----
`Sys.Coll.Imm.List` | `100` | `20 us` | `6,016 ns` | `1,021 ns`
`Sys.Coll.Imm.List` | `1000` | `342 us` | `62,300 ns` | `32,942 ns`
`Sys.Coll.Imm.List` | `10000` | `5,942 us` | `605,456 ns` | `443,482 ns`
`Sys.Coll.Imm.List` | `100000` | `87,089 us` | `8,154,876 ns` | `6,146,402 ns`
`LanguageExt.Lst` | `100` | `17 us` | `1,386 ns` | `1,213 ns`
`LanguageExt.Lst` | `1000` | `273 us` | `13,455 ns` | `35,304 ns`
`LanguageExt.Lst` | `10000` | `5,341 us` | `137,278 ns` | `471,614 ns`
`LanguageExt.Lst` | `100000` | `62,340 us` | `1,696,505 ns` | `6,195,590 ns`
`LanguageExt.Seq` | `100` | `2 us` | `638 ns` | `359 ns`
`LanguageExt.Seq` | `1000` | `17 us` | `6,194 ns` | `3,518 ns`
`LanguageExt.Seq` | `10000` | `205 us` | `61,358 ns` | `35,117 ns`
`LanguageExt.Seq` | `100000` | `1,879 us` | `613,746 ns` | `351,036 ns`

### Lists with reference types

Type | N | Add | RandomAccess | Iteration
----- | ----- | ----- | ----- | -----
`Sys.Coll.Imm.List` | `100` | `22 us` | `10,652 ns` | `1,013 ns`
`Sys.Coll.Imm.List` | `1000` | `357 us` | `105,946 ns` | `33,410 ns`
`Sys.Coll.Imm.List` | `10000` | `6,237 us` | `1,069,881 ns` | `448,148 ns`
`Sys.Coll.Imm.List` | `100000` | `83,450 us` | `13,386,415 ns` | `6,260,259 ns`
`LanguageExt.Lst` | `100` | `24 us` | `1,547 ns` | `3,561 ns`
`LanguageExt.Lst` | `1000` | `349 us` | `14,446 ns` | `56,720 ns`
`LanguageExt.Lst` | `10000` | `6,240 us` | `148,034 ns` | `747,441 ns`
`LanguageExt.Lst` | `100000` | `75,870 us` | `1,986,228 ns` | `9,194,727 ns`
`LanguageExt.Seq` | `100` | `3 us` | `888 ns` | `856 ns`
`LanguageExt.Seq` | `1000` | `28 us` | `8,489 ns` | `8,537 ns`
`LanguageExt.Seq` | `10000` | `367 us` | `84,479 ns` | `85,092 ns`
`LanguageExt.Seq` | `100000` | `4,463 us` | `856,149 ns` | `853,446 ns`

## Maps

### Unsorted maps with value types

Type | N | Add | ContainsKey | Iterate | RandomRemoval
----- | ----- | ----- | ----- | ----- | -----
`Sys.Coll.Imm.Dictionary` | `100` | `69 us` | `2,348 ns` | `10,647 ns` | `49,871 ns`
`Sys.Coll.Imm.Dictionary` | `1000` | `1,125 us` | `60,126 ns` | `118,251 ns` | `894,815 ns`
`Sys.Coll.Imm.Dictionary` | `10000` | `17,785 us` | `958,715 ns` | `1,164,131 ns` | `13,768,680 ns`
`Sys.Coll.Imm.Dictionary` | `100000` | `298,988 us` | `17,859,368 ns` | `13,715,622 ns` | `242,798,528 ns`
`LanguageExt.HashMap` | `100` | `15 us` | `2,042 ns` | `3,579 ns` | `14,065 ns`
`LanguageExt.HashMap` | `1000` | `245 us` | `27,338 ns` | `38,108 ns` | `231,652 ns`
`LanguageExt.HashMap` | `10000` | `5,023 us` | `354,385 ns` | `459,233 ns` | `3,810,743 ns`
`LanguageExt.HashMap` | `100000` | `92,372 us` | `7,196,852 ns` | `7,432,346 ns` | `77,218,736 ns`

### Unsorted maps with reference types

Type | N | Add | ContainsKey | Iterate | RandomRemoval
----- | ----- | ----- | ----- | ----- | -----
`Sys.Coll.Imm.Dictionary` | `100` | `90 us` | `6 us` | `20,352 ns` | `67 us`
`Sys.Coll.Imm.Dictionary` | `1000` | `1,481 us` | `104 us` | `202,502 ns` | `1,085 us`
`Sys.Coll.Imm.Dictionary` | `10000` | `23,336 us` | `1,504 us` | `2,076,426 ns` | `17,480 us`
`Sys.Coll.Imm.Dictionary` | `100000` | `390,738 us` | `28,854 us` | `26,252,578 ns` | `291,299 us`
`LanguageExt.HashMap` | `100` | `27 us` | `5 us` | `5,394 ns` | `24 us`
`LanguageExt.HashMap` | `1000` | `404 us` | `74 us` | `62,036 ns` | `364 us`
`LanguageExt.HashMap` | `10000` | `7,080 us` | `859 us` | `755,971 ns` | `5,773 us`
`LanguageExt.HashMap` | `100000` | `138,283 us` | `17,573 us` | `12,049,411 ns` | `114,837 us`

### Sorted maps with value types

Type | N | Add | ContainsKey | Iterate | RandomRemoval
----- | ----- | ----- | ----- | ----- | -----
`Sys.Coll.Imm.SortedDictionary` | `100` | `31 us` | `3,814 ns` | `6,140 ns` | `24,785 ns`
`Sys.Coll.Imm.SortedDictionary` | `1000` | `539 us` | `78,032 ns` | `61,408 ns` | `449,868 ns`
`Sys.Coll.Imm.SortedDictionary` | `10000` | `9,631 us` | `1,165,587 ns` | `617,841 ns` | `7,246,961 ns`
`Sys.Coll.Imm.SortedDictionary` | `100000` | `203,653 us` | `19,308,708 ns` | `8,107,066 ns` | `149,215,888 ns`
`LanguageExt.Map` | `100` | `19 us` | `2,805 ns` | `1,593 ns` | `13,458 ns`
`LanguageExt.Map` | `1000` | `335 us` | `82,493 ns` | `16,605 ns` | `283,067 ns`
`LanguageExt.Map` | `10000` | `6,871 us` | `1,272,800 ns` | `190,544 ns` | `5,016,271 ns`
`LanguageExt.Map` | `100000` | `155,131 us` | `22,845,338 ns` | `3,707,917 ns` | `118,625,112 ns`

### Sorted maps with reference types

Type | N | Add | ContainsKey | Iterate | RandomRemoval
----- | ----- | ----- | ----- | ----- | -----
`Sys.Coll.Imm.SortedDictionary` | `100` | `79 us` | `42 us` | `10,449 ns` | `59 us`
`Sys.Coll.Imm.SortedDictionary` | `1000` | `1,333 us` | `706 us` | `103,676 ns` | `1,069 us`
`Sys.Coll.Imm.SortedDictionary` | `10000` | `21,790 us` | `10,429 us` | `1,073,453 ns` | `17,848 us`
`Sys.Coll.Imm.SortedDictionary` | `100000` | `393,881 us` | `164,318 us` | `13,811,388 ns` | `319,229 us`
`LanguageExt.Map` | `100` | `72 us` | `44 us` | `2,112 ns` | `51 us`
`LanguageExt.Map` | `1000` | `1,137 us` | `737 us` | `22,033 ns` | `896 us`
`LanguageExt.Map` | `10000` | `19,178 us` | `10,676 us` | `252,819 ns` | `15,598 us`
`LanguageExt.Map` | `100000` | `350,295 us` | `171,571 us` | `4,256,098 ns` | `301,462 us`

## Sets

### Unsorted sets with value types

Type | N | Add | Contains | Iteration | RandomRemoval
----- | ----- | ----- | ----- | ----- | -----
`Sys.Coll.Imm.HashSet` | `100` | `52 us` | `3 us` | `11,975 ns` | `39,490 ns`
`Sys.Coll.Imm.HashSet` | `1000` | `878 us` | `65 us` | `118,770 ns` | `702,550 ns`
`Sys.Coll.Imm.HashSet` | `10000` | `14,230 us` | `1,033 us` | `1,245,373 ns` | `11,267,592 ns`
`Sys.Coll.Imm.HashSet` | `100000` | `249,189 us` | `17,250 us` | `15,266,368 ns` | `207,453,344 ns`
`LanguageExt.HashSet` | `100` | `13 us` | `2 us` | `3,366 ns` | `13,782 ns`
`LanguageExt.HashSet` | `1000` | `224 us` | `23 us` | `34,842 ns` | `221,730 ns`
`LanguageExt.HashSet` | `10000` | `4,520 us` | `320 us` | `458,131 ns` | `3,744,864 ns`
`LanguageExt.HashSet` | `100000` | `85,851 us` | `5,648 us` | `6,528,404 ns` | `74,686,456 ns`

### Unsorted sets with reference types

Type | N | Add | Contains | Iteration | RandomRemoval
----- | ----- | ----- | ----- | ----- | -----
`Sys.Coll.Imm.HashSet` | `100` | `66 us` | `8 us` | `26,331 ns` | `51 us`
`Sys.Coll.Imm.HashSet` | `1000` | `1,008 us` | `117 us` | `261,931 ns` | `833 us`
`Sys.Coll.Imm.HashSet` | `10000` | `15,992 us` | `1,575 us` | `2,660,329 ns` | `12,879 us`
`Sys.Coll.Imm.HashSet` | `100000` | `271,162 us` | `25,866 us` | `30,410,462 ns` | `224,692 us`
`LanguageExt.HashSet` | `100` | `21 us` | `6 us` | `3,665 ns` | `21 us`
`LanguageExt.HashSet` | `1000` | `322 us` | `69 us` | `40,037 ns` | `327 us`
`LanguageExt.HashSet` | `10000` | `5,761 us` | `820 us` | `488,167 ns` | `4,944 us`
`LanguageExt.HashSet` | `100000` | `113,138 us` | `13,739 us` | `7,971,738 ns` | `94,890 us`

### Sorted sets with value types

Type | N | Add | Contains | Iteration | RandomRemoval
----- | ----- | ----- | ----- | ----- | -----
`Sys.Coll.Imm.SortedSet` | `100` | `29 us` | `3 us` | `6,058 ns` | `23,319 ns`
`Sys.Coll.Imm.SortedSet` | `1000` | `506 us` | `67 us` | `60,265 ns` | `425,159 ns`
`Sys.Coll.Imm.SortedSet` | `10000` | `8,748 us` | `1,009 us` | `618,772 ns` | `6,950,004 ns`
`Sys.Coll.Imm.SortedSet` | `100000` | `195,510 us` | `17,397 us` | `7,341,612 ns` | `145,175,136 ns`
`LanguageExt.Set` | `100` | `17 us` | `3 us` | `1,575 ns` | `18,006 ns`
`LanguageExt.Set` | `1000` | `306 us` | `70 us` | `16,889 ns` | `324,040 ns`
`LanguageExt.Set` | `10000` | `6,199 us` | `1,101 us` | `192,437 ns` | `5,918,020 ns`
`LanguageExt.Set` | `100000` | `147,726 us` | `20,893 us` | `2,980,587 ns` | `138,141,008 ns`

### Sorted sets with reference types

Type | N | Add | Contains | Iteration | RandomRemoval
----- | ----- | ----- | ----- | ----- | -----
`Sys.Coll.Imm.SortedSet` | `100` | `72 us` | `40 us` | `10,645 ns` | `60 us`
`Sys.Coll.Imm.SortedSet` | `1000` | `1,187 us` | `683 us` | `104,584 ns` | `1,022 us`
`Sys.Coll.Imm.SortedSet` | `10000` | `19,371 us` | `9,842 us` | `1,071,031 ns` | `16,301 us`
`Sys.Coll.Imm.SortedSet` | `100000` | `329,705 us` | `161,498 us` | `13,185,054 ns` | `288,019 us`
`LanguageExt.Set` | `100` | `63 us` | `37 us` | `1,644 ns` | `47 us`
`LanguageExt.Set` | `1000` | `1,001 us` | `629 us` | `17,305 ns` | `854 us`
`LanguageExt.Set` | `10000` | `16,150 us` | `9,435 us` | `200,705 ns` | `14,227 us`
`LanguageExt.Set` | `100000` | `307,030 us` | `144,921 us` | `2,834,565 ns` | `272,077 us`


