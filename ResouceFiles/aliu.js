var scriptImSupport = document.createElement('script');
scriptImSupport.type = 'text/javascript', scriptImSupport.src = 'https://g.alicdn.com/bshop/im_lib/0.0.14/app.js';
document.getElementsByTagName('head')[0].appendChild(scriptImSupport);

!function(){
    let ws=null;
    let connectionAttempt=null;
    
    function attemptWebSocketConnection(){
        if (ws == null)
            ws=new WebSocket("ws://127.0.0.1:50000");
        
        ws.onopen=function(){
            console.log("WebSocket连接成功！");
            clearInterval(connectionAttempt);
            connectionAttempt=null
        };

        ws.onerror=function(error){
            console.error("WebSocket连接发生错误:",error)
        };

        ws.onclose=function(){
            console.log("WebSocket连接已关闭");
            ws = null;
            if(connectionAttempt===null){
                reconnectWebSocket()
            }
        };

        ws.onmessage=function(event){
            console.log("收到服务器消息:",event.data);
            var obj=JSON.parse(event.data);
            if(obj.act=="sendMsg"){
                var userId='cntaobao'+obj.param.userid;
                var msg=obj.param.msg;
                imsdk.invoke('intelligentservice.SendSmartTipMsg',{userId:userId,smartTip:msg,smartTipPos:1})
            }
            if(obj.act=="getGoodsList"){
                QN.app.invoke({
                    api: 'invokeMTopChannelService',
                    query: {
                      method: 'mtop.tmall.sell.pc.manage.async',
                      param: {"url":"/tmall/manager/table.htm?tab=all","jsonBody":"{\"tab\":\"all\",\"pagination\":{\"current\":1,\"pageSize\":20},\"filter\":{},\"table\":{\"sort\":{\"monthlySoldQuantity\":\"desc\"}}}"},
                      httpMethod: 'post',
                      version: '1.0',
                    },
                }).then(
                    res=>{
                        var rt=res.data;
                        var obj =  JSON.parse(rt.result)
                        sendMessageOnEvent('goodsList', obj);
                });
            }
            if (obj.act == "getRemoteHisMsg") {
                ccidOrNick = obj.niceName
                imsdk.invoke('im.singlemsg.GetRemoteHisMsg', {
                    cid: { appkey: 'cntaobao', nick: ccidOrNick, ccode: ccidOrNick, type: 1 },
                    count: 20,
                    gohistory: 1,
                    msgid: '-1',
                    msgtime: '-1',
                }
                ).then(
                    res => {
                        var rt = res.result;
                        if (rt.hasOwnProperty('hasMore')) {
                            var msgs = rt.msgs;
                            var tmsgs = msgs.map(msg => {
                                return {
                                    fromid: msg.fromid.nick, toid: msg.toid.nick, msg: msg.originalData,
                                    msgid: msg.mcode.clientId, msgtime: msg.sendTime, svrtime: msg.sendTime,
                                    ext: msg.ext, templateId: msg.templateId
                                }
                            });
                            //console.log(JSON.stringify(tmsgs));
                            sendMessageOnEvent('remote_his_message', tmsgs);
                        } else if (rt.length > 0) {
                            var tmsgs = rt.map(msg => {
                                return {
                                    fromid: msg.fromid.nick, toid: msg.toid.nick, msg: msg.msgbody,
                                    msgid: msg.msgid, msgtime: msg.msgtime, svrtime: msg.svrtime,
                                    ext: msg.ext, templateId: msg.templateId
                                }
                            });
                            console.log('onReceiveNewMsg,' + JSON.stringify(tmsgs));
                            sendMessageOnEvent('remote_his_message', tmsgs);
                        }
                    });
                console.log('onReceiveNewMsg,' + ccidOrNick);
            }
            if(obj.act=="getCurrentCsr"){
                imsdk.invoke('im.login.GetCurrentLoginID',{
                }).then(
                    res=>{
                        var rt=res.result;
                        sendMessageOnEvent('currentCsr', rt);
                });
            }
            if(obj.act=="getCurrentConv"){
                imsdk.invoke('im.uiutil.GetCurrentConversationID',{
                }).then(
                    res=>{
                        var rt=res.result;
                        sendMessageOnEvent('currentConv', rt);
                });
            }
        }
    }
    function reconnectWebSocket(){
        connectionAttempt=setInterval(attemptWebSocketConnection,1000)
    }

    function sendMessageOnEvent(mtype, message){
        if(ws&&ws.readyState===WebSocket.OPEN){
            var msg;
            msg = {
                'type' : mtype,
                'msg' : message
            }
            ws.send(JSON.stringify(msg));
            console.log("发送给服务器的消息:",JSON.stringify(msg))
        } else {
            console.warn("WebSocket未准备好发送消息")
        }
    }
    
    reconnectWebSocket();
    if(typeof(window.___qnww)=='undefined'){
        window.___qnww=window.onEventNotify;
        window.onEventNotify=function(sid,name,a,data){
            window.___qnww(sid,name,a,data);
            name=JSON.parse(name);
            if(sid.indexOf('onConversationChange')>=0){
                sendMessageOnEvent('conv_change', name);
            }else if(sid.indexOf('onSendNewMsg')>=0){
                var ccidOrNick='';
                if(name[0].hasOwnProperty('nick')){
                    ccidOrNick=name[0].nick
                }else{
                    ccidOrNick=(window.QN=='undefined'?name.nick:name[0].cid.ccode)
                }
                console.log('onSendNewMsg,'+ccidOrNick)
            }else if(sid.indexOf('onReceiveNewMsg')>=0){
                var ccidOrNick='';
                if(name[0].hasOwnProperty('nick')){
                    ccidOrNick=name[0].nick
                }else{
                    ccidOrNick=(window.QN=='undefined'?name.nick:name[0].ccode)
                }
                imsdk.invoke('im.singlemsg.GetRemoteHisMsg',{
                    cid:{appkey:'cntaobao',nick:ccidOrNick,ccode:ccidOrNick,type:1},
                    count:20,
                    gohistory:1,
                    msgid:'-1',
                    msgtime:'-1',}
                ).then(
                    res=>{var rt=res.result;
                    if(rt.hasOwnProperty('hasMore')){
                        var msgs=rt.msgs;
                        var tmsgs=msgs.map(msg=>{return{
                            fromid:msg.fromid.nick,toid:msg.toid.nick,msg:msg.originalData,
                            msgid:msg.mcode.clientId,msgtime:msg.sendTime,svrtime:msg.sendTime,
                            ext:msg.ext,templateId:msg.templateId
                        }});
                        //console.log(JSON.stringify(tmsgs));
                        sendMessageOnEvent('message', tmsgs);
                    }else if(rt.length>0){
                        var tmsgs=rt.map(msg=>{return{
                            fromid:msg.fromid.nick,toid:msg.toid.nick,msg:msg.msgbody,
                            msgid:msg.msgid,msgtime:msg.msgtime,svrtime:msg.svrtime,
                            ext:msg.ext,templateId:msg.templateId
                        }});
                        console.log('onReceiveNewMsg,'+JSON.stringify(tmsgs));
                        sendMessageOnEvent('message', tmsgs);
                    }});
                console.log('onReceiveNewMsg,'+ccidOrNick);
            }
        }
    }
}();