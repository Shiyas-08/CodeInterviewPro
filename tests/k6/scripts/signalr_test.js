import ws from 'k6/ws';
import { check } from 'k6';

export const options = {
    vus: 50,
    duration: '1m',
    insecureSkipTLSVerify: true,
};

export default function () {
    const url = 'wss://localhost:7000/interviewHub';
    const params = { tags: { my_tag: 'signalr_test' } };

    const res = ws.connect(url, params, function (socket) {
        socket.on('open', () => {
            console.log('connected to signalr');
            
            // SignalR Handshake
            socket.send(JSON.stringify({ protocol: 'json', version: 1 }) + '\u001e');
        });

        socket.on('message', (data) => {
            // SignalR messages end with the record separator char
            if (data.includes('\u001e')) {
                const msg = data.split('\u001e')[0];
                // console.log('Message received: ' + msg);
            }
        });

        socket.on('close', () => console.log('disconnected'));
        
        socket.setTimeout(function () {
            socket.close();
        }, 30000); // Close after 30s
    });

    check(res, { 'status is 101': (r) => r && r.status === 101 });
}
