## 简介

本章总结了各位开发者在开发中容易出现的一些问题及解决方案. 为开发者提供快速问题查询通道.

同时也欢迎开发者通过[Issues](https://github.com/Jie2GG/Native.Framework/issues)或者[QQ群](http://qm.qq.com/cgi-bin/qm/qr?k=HT_Ei-HIuCnVRIyMzB-CQQXeF80ZyUC-&group_code=711841640)进行反馈

## 常见错误

> 为什么酷Q日志上显示已发送消息, 但是QQ上未显示消息

答: 酷Q属于第三方客户端, 发送的消息可能被TX风控, 请在服务器上使用官方客户端挂一段时间并绑定设备锁. 然后再使用酷Q

> 为什么生成结果只有 app.dll 没有 app.json

答: 请查看 **Native.Core** 项目中的 **app.json** 的属性: "始终复制" 是否已为 True

> 为什么突然在使用 **获取xxxx列表**、**获取xxxx信息** 的过程中突然报错?

答: 因为SDK是从酷Q获取的数据, 而这些数据并不是 100% 正确的, 所以SDK在 "Debug" 模式下对酷Q返回的数据进行了校验, 以方便开发者确定错误的源头. 开发者应在开发中对返回的结果进行 null 判断, 在切换回 "Release" 模式后, SDK将消除这些异常的代码. 除错后将返回 null 值.

* 其它问题正在收集中...