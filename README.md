# ChatSignalR

I have to finish the front...

![image](https://user-images.githubusercontent.com/49936550/212996040-150b8da5-5861-47a8-9607-17be1a929403.png)
![image](https://user-images.githubusercontent.com/49936550/212996404-c54cb867-ee07-42cf-a69f-e3b0b51dd31f.png)
![image](https://user-images.githubusercontent.com/49936550/212996515-218e2fe9-83bd-48c7-815b-d2ad7d58f02a.png)


DEBUG SCHEMA:

mutation{
  register {
    register(request: { 
      firstName: "david" 
      lastName: "Dato" 
      login:"login" 
      mobileOrEmail:"24352345" 
      password:"Superpass_1"}) {
      errors
    }
  }
}

mutation {
  chat {
    createPrivateChat(otherUserId: 4) {
      chatId name userIds
    }
  }
}

mutation {
  chat {
    sendMessageToChat(text: "Hi man!" chatId: 1) {
      messageId senderName text sendTime
    }
  }
}

query {
  users {
    users(request: { chatIdFilter: [1,2,3,4,5] fullNameFilter: "David" idFilter: [1,2,3,4,5] phoneOrMailFilter: ""}) {
       id firstName lastName
    }
  }
}

query {
  chat {
    chats(request: { onlyUserChats: true}) { name id chatUsers { userId firstName}}
  }
}
