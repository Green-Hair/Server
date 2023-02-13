# 协议

## 操作：发送邮件验证码（sendemail）
* 请求字段：email
* 返回：waitNumber

## 操作：登录(login)
* 请求字段：id,psw,captcha
* 返回：name,token
* token为-1代表登陆失败

## 操作：注册(enroll)
* 请求字段：name,psw,captcha
* 返回：id,token
* token为-1代表注册失败

## 操作：发送信息(send)
* 请求字段：token,to_id,msg
* 返回：status
* to_id定向到某一私人群聊则是群发到某一个私人群聊
* 如果to_id字段为空那么则发送至官方聊天室