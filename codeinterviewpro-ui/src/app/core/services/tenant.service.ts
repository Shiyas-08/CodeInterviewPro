import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class TenantService {
  private apiUrl = `${environment.apiUrl}/tenants`;

  constructor(private http: HttpClient) { }

  getAll() {
    return this.http.get<any>(this.apiUrl);
  }

  getById(id: string) {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }

  create(tenant: any) {
    return this.http.post<any>(this.apiUrl, tenant);
  }

  update(id: string, tenant: any) {
    return this.http.put<any>(`${this.apiUrl}/${id}`, tenant);
  }

  delete(id: string) {
    return this.http.delete<any>(`${this.apiUrl}/${id}`);
  }
}
