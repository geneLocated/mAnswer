# 匹配答案

我用于参加知识竞赛写的自动答题工具，现该竞赛已结束。  
排名按理位于全市第一，比第二名快 20 多秒。不过因为未知的原因，成绩未进入数据库。  
当时我还写了一个半自动的安卓版本，请见 [android](https://github.com/genelocated/mAnswer/tree/android) 分支。

可通过 [MSBuild](https://www.microsoft.com/download/details.aspx?id=48159) 微软编译工具或 Visual Studio 编译。  

```Shell
setx path 'C:\Program Files (x86)\MSBuild\14.0\Bin'
git clone https://github.com/genelocated/manswer
cd manswer
msbuild
```

生成的程序可在 `.\匹配答案\bin` 下找到。  

![Screenshot](https://user-images.githubusercontent.com/31200881/38315003-1caf024c-385a-11e8-8846-d26cb3438ab0.PNG)

## 许可协议

[MIT](./LICENSE)
