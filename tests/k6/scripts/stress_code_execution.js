import http from 'k6/http';
import { check, sleep } from 'k6';
import { BASE_URL, login } from '../modules/auth.js';

export const options = {
    scenarios: {
        code_runners: {
            executor: 'constant-vus',
            vus: 10,
            duration: '1m',
        },
    },
    thresholds: {
        'http_req_duration': ['p(99)<5000'], // Code execution can be slow, 5s limit
    },
    insecureSkipTLSVerify: true,
};

// Setup runs once per test
export function setup() {
    const token = login('admin@system.com', 'Admin@123');
    return { token: token };
}

export default function (data) {
    if (!data.token) return;

    const payload = JSON.stringify({
        language: 1, // CSharp
        code: 'using System; Console.WriteLine("Hello from k6 load test!");',
        testCases: [],
        methodName: 'Main'
    });

    const params = {
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${data.token}`,
            'Cookie': `accessToken=${data.token}`
        },
    };

    const res = http.post(`${BASE_URL}/CodeExecution`, payload, params);

    const success = check(res, {
        'execution successful': (r) => r.status === 200,
        'has feedback': (r) => r.json().aiFeedback !== undefined,
    });

    if (!success) {
        console.error(`Execution failed: Status=${res.status}, Body=${res.body}`);
    }

    sleep(2); // Wait between submissions
}
