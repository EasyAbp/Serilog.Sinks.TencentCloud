# Serilog.Sinks.TencentCloud

## How to use it

```
Log.Logger = new LoggerConfiguration()
                .WriteTo.TencentCloud("请求域名(ap-guangzhou.cls.myqcloud.com)", "topic_id(b8c1fafe-677a-4cc2-9c26-d962d5caa077)", "TencentCloud API Sercet Id", "TencentCloud API Sercet Key", restrictedToMinimumLevel: LogEventLevel.Warning)
                .CreateLogger()
```

Used in conjunction with Serilog.Settings.Configuration the same sink can be configured in the following way:

```
{
  "Serilog": {
    "MinimumLevel": "Verbose",
    "WriteTo": [
      {
        "Name": "TencentCloud",
        "Args": {
          "requestBaseUri": "ap-guangzhou.cls.myqcloud.com",
          "topicId": "",
          "secretId": "",
          "secretKey": ""
        }
      }
    ]
  }
}
```