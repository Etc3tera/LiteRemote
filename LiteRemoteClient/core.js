$.connection.hub.url = "http://192.168.1.169:54321/signalr";

var remote = $.connection.myRemoteHub;
var myApp = new App({ fps: 12 });
var app = new Vue({
    el: '#app',
    data: {
        screen: 'data:image/gif;base64,R0lGODlhAQABAIAAAAAAAP///yH5BAEAAAAALAAAAAABAAEAAAIBRAA7',
        keySequence: '',
        keyMessage: ''
    },
    methods: {
        sendLeftClick: function(event){
            setTimeout(function(){
                remote.server.l_Click(event.offsetX, event.offsetY);
            }, 100);
        },
        sendRightClick: function(event){
            remote.server.r_Click(event.offsetX, event.offsetY);
        },
        sendKeySequence: function(){
            remote.server.sequenceKeyPress(this.keySequence);
        },
        sendKeyMessage: function(){
            remote.server.messageKeyPress(this.keyMessage);
        }
    }
});

remote.client.foo = function(){

};

$.connection.hub.start().done(function () {
    myApp.Initialize();
}).fail(error => {
    alert('error to connect 2');
});

function App(options){
    var self = this;
    var stillLoading = false;

    this.Initialize = function(){
        setInterval(function(){
            if(!stillLoading){
                RefreshScreen();
            }
        }, 1000 / options.fps);
    }
    
    // Private functions
    function RefreshScreen(){
        var promise = remote.server.refresh();
        stillLoading = true;
        promise.done(data => {
            app.screen = data;
            stillLoading = false;
        });
    }    
}


