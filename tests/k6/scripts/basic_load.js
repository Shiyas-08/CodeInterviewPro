import http from 'k6/http';
import { check, sleep } from 'k6';

export const BASE_URL = 'https://localhost:7000/api';

export const options = {
    stages: [
        { duration: '30s', target: 20 }, // ramp up to 20 users
        { duration: '1m', target: 20 },  // stay at 20 users
        { duration: '30s', target: 0 },  // ramp down to 0 users
    ],
    insecureSkipTLSVerify: true, // Skip SSL certificate validation for localhost
    thresholds: {
        http_req_duration: ['p(95)<500'], // 95% of requests must complete below 500ms
        http_req_failed: ['rate<0.01'],   // less than 1% failure rate
    },
};

export default function () {
    // Test a valid endpoint (this might return 401 Unauthorized but is a valid route)
    const res = http.get(`${BASE_URL}/dashboard/summary`); 
    
    check(res, {
        'is not 404': (r) => r.status !== 404,
        'response time < 500ms': (r) => r.timings.duration < 500,
    });

    sleep(1);
}
