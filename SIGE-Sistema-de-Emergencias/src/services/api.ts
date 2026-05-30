// src/services/api.js
import axios from 'axios';

const api = axios.create({
  baseURL: 'https://localhost:7057', 
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json',
    'Authorization': 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJtdXJpbG9iZXJuYXJkb0BnbWFpbC5jb20iLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjJkOGE4OWUxLTA1ZjQtNDBhZC1iYWY3LWFiMWZhZDkwNmM0NCIsImp0aSI6IjhkMzU3Y2U5LWIzNTMtNGRiMi05MzM4LTNiNzg3ZjBkNTE0NyIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkFkbWluIiwiZXhwIjoxNzgwMzI2MjAzLCJpc3MiOiJTSUdFQXBpIiwiYXVkIjoiU0lHRVVzZXJzIn0.lBCYVsXXbtXfBj0XykB2fvgjh8mJds-v1vG39c4jjYc'
  }
});

export default api;