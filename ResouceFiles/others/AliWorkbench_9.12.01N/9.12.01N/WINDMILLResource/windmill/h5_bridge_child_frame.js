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
            queue: JSON.stringify([func]),
        }));
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
