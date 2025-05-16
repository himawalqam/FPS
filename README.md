整理我开发期间遇到的问题和BUG
1.遇到着色器URL问题:(1)选择已经混乱的material(一般是全变粉色), Edit->Rendering->Materials->Convert Selected Built-in Materials to URP(如果这个行不通使用第二种)
                  (2)Window->Rendering->Render Pipeline Converter, 选择Rendering Settings,Material Upgrade,Readonly Material Converter. 然后点击Convert Assets再点击Initialize And Convert. 这样就能解决着色器问题了
    
2.Outline无法被取消: 如果有物品Outline, 并且有粒子系统的话大概率会出现粒子系统混乱而我们的做的特效(例如开枪时枪口出现的烟火)无法正常运行(只在编辑器里是这样, 打包以后不会这样了), 这个时候我们在Update函数或者拾取物品的时候关闭outline即可.
3.HUNDManager无法正常工作: 如果Resources文件夹和Resources.Load<GameObject>("Pistol1911_Weapon")里的文件名确定是一样并且挂在脚本上的物品是正确的那么把GetComponentInChildren<>写成了GetComponent<>, 这两个的区别就是写成了GetComponent只会在本物体中寻找物件,GetComponentInChildren会在子物体中也会寻找物件

