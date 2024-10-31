# AiPictureDetector
支持两个接口，[HiveAI](https://hivemoderation.com/ai-generated-content-detection?scroll=demo) [SightEngine](https://dashboard.sightengine.com/)
- HiveAI未公开接口，每次使用时请去[Demo](https://thehive.ai/demos)手动过一下验证码，每次验证可能能用半天？
- SightEngine每月免费2000次调用，注册即可

## SightEngine 鉴权
前往[Dashboard](https://dashboard.sightengine.com/api-credentials)获取`APIUser`与`APISecret`，填入插件相关配置中

## 配置
| 键                                 | 描述                                       | 默认值                                      |
|----------------------------------|------------------------------------------|-------------------------------------------|
|CommandMenu|指令触发文本|#鉴癌|
|ReplyIsAI|AI图概率高的文本|像啊，很像啊\n是 AI 的概率为 {0}%|
|ReplyNotAI|不是AI图概率高的文本|不太像\n不是 AI 的概率为 {0}%|
|SightEngine_UserID|SightEngine的APIUser||
|SightEngine_UserSecret|SightEngine的APISecret||
|DetectorType|接口类型（0=HiveAI 1=SightEngine）|0|

## 示例
![image](https://github.com/user-attachments/assets/d5507ede-c2dc-424d-b53a-ac78d7fb11cb)
