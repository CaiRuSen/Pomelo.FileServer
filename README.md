.Net Core 3.1 文件上传 Web API 服务 <br/>

功能说明：
<br/>
1.权限校验（AppId，APPsecret | appsettings.json 中 Auth 配置）
<br/>
2.文件上传（支持格式校验和文件格式限制 | appsettings.json 中 ContentType 配置）
<br/>
3.图片文件处理（压缩，等比裁剪，添加水印）
<br/>
4.上传位置（支持服务器本地存储，阿里云OSS，七牛云存储，AWS S3 | appsettings.json 中 UploadConfig 配置）
<br/>
这里要注意，如果将文件存储到服务器本地，需要做好相关安全策略，比如限制某些格式的文件上传，上传的文件夹做好权限控制，比如不提供执行文件的权限，有条件的还可以先对上传的内容运行病毒/恶意软件扫描程序，然后再存储文件
<br/>
5.接口限流控制（基于 AspNetCoreRateLimit | appsettings.json 中 ClientRateLimiting 配置）
<br/>

接口文档：{host}/doc
