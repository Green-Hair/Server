# 协议

## 操作：登录(login)
* 请求字段：id,psw
* 返回：name,token
* token为-1代表登陆失败

## 操作：注册(logon)
* 请求字段：name,psw
* 返回：id,token
* token为-1代表注册失败

## 操作：发送信息(send)
* 请求字段：token,to_id,msg
* 返回：status

## 操作：