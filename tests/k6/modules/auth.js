import http from 'k6/http';
import { check } from 'k6';

export const BASE_URL = 'https://localhost:7000/api';

export function login(email, password) {
    const payload = JSON.stringify({
        email: email,
        password: password,
    });

    const params = {
        headers: {
            'Content-Type': 'application/json',
        },
    };

    const res = http.post(`${BASE_URL}/auth/login`, payload, params);

    const success = check(res, {
        'login successful': (r) => r.status === 200,
        'has cookie': (r) => r.cookies.accessToken !== undefined,
    });

    if (!success) {
        console.error(`Login failed for ${email}: Status=${res.status}, Body=${res.body}`);
        return null;
    }

    return res.cookies.accessToken[0].value;
}
