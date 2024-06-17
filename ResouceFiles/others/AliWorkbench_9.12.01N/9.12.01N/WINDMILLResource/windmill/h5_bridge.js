const callbackPoll = {};
const idDebugMode = /__performance=true/.test(location.href);
const JSAPI = {
    call(func, param, callback) {
        if ('string' !== typeof func) {
            return;
        }

        if ('function' === typeof param) {
            callback = param;
            param = null;
        } else if (typeof param !== 'object') {
            param = null;
        }

        const callbackId = func + '_' + new Date().getTime() + (Math.random());
        if ('function' === typeof callback) {
            callbackPoll[callbackId] = callback;
        }

        if (param && param.callbackId) {
            func = {
                responseId: param.callbackId,
                responseData: param
            };
            delete param.callbackId;
        } else {
            func = {
                handlerName: func,
                data: JSON.stringify(param) || ''
            };
            func.callbackId = '' + callbackId;

            if (idDebugMode && func.handlerName === 'message') {
                console.log(`[performance] ::render::PostMessage:: size: ${JSON.stringify([func]).length}  timestamp: ${Date.now()}  content: `);
            }
        }


        windmill.postMessage(JSON.stringify({
            type: 'api',
            queue: JSON.stringify([func])
        }));
    },
    trigger(name, data) {

        if (!name) {
            return;
        }
        const triggerEvent = (name, data) => {
            let callbackId;
            if (data && data.callbackId) {
                callbackId = data.callbackId;
                data.callbackId = null;
            }
            const evt = document.createEvent('Events');
            evt.initEvent(name, false, true);
            evt.syncJsApis = [];

            if (data) {
                if (data.__pull__) {
                    delete data.__pull__;
                    for (let k in data) {
                        evt[k] = data[k];
                    }
                } else {
                    evt.data = data;
                }
            }
            const canceled = !document.dispatchEvent(evt);
            if (callbackId) {
                const callbackData = {};
                callbackData.callbackId = callbackId;
                callbackData[name + 'EventCanceled'] = canceled;
                callbackData['syncJsApis'] = evt.syncJsApis;
                this.call('__nofunc__', callbackData);
            }
        };
        triggerEvent(name, data);
    },
    _handleMessageFromObjC(response) {

        const resp = typeof response === 'string' ? JSON.parse(response) : response;

        if (resp.responseId) {
            const func = callbackPoll[resp.responseId];
            if (!(typeof resp.keepCallback == 'boolean' && resp.keepCallback)) {
                delete callbackPoll[resp.responseId];
            }
            if ('function' === typeof func) {
                func(resp.responseData);
            }
        } else if (resp.handlerName) {

            if (idDebugMode && resp.handlerName === 'message') {
                if (!resp.data || !resp.data.data) {
                    return;
                }
                let data = resp.data.data;
                console.log(`[performance] ::render::RecevieMessage:: size: ${data.length}  timestamp: ${Date.now()}  content: `);
            }

            if (resp.callbackId) {
                resp.data = resp.data || {};
                resp.data.callbackId = resp.callbackId;
            }
            this.trigger(resp.handlerName, resp.data);
        }
    }
}

window.AlipayJSBridge = JSAPI;

// const init = () => {
//     var readyEvent = document.createEvent('Events');
//     readyEvent.initEvent('AlipayJSBridgeReady', false, false);
//     document.dispatchEvent(readyEvent);
// }

// document.addEventListener('DOMContentLoaded', init);
document.addEventListener('AlipayJSBridgeReady', evt => {
    console.log('%c [listener::AlipayJSBridgeReady]', 'background: #e2c072; color: #fff', evt);
});
