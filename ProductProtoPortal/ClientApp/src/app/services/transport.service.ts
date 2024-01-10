import {Injectable} from '@angular/core';
import {catchError, map, Observable, Subject, timeout} from "rxjs";
import {HttpClient, HttpEvent, HttpEventType, HttpHeaders, HttpParams, HttpRequest} from "@angular/common/http";
import {ApiAnswer} from "../model/ApiAnswer";

@Injectable({
  providedIn: 'root'
})
export class TransportService {

  private timeout = 25000;
  private timeOutSub: Subject<string> = new Subject<string>();
  private errorSub: Subject<string> = new Subject<string>();
  get OnError(): Observable<string> {
    return this.errorSub.asObservable();
  }

  get OnTimeout(): Observable<string> {
    return this.timeOutSub.asObservable();
  }
  constructor(private http:HttpClient) {
  }
  private GetHeaders():HttpHeaders{
    let headers: HttpHeaders = new HttpHeaders();
    headers = headers.append("Content-type", "application/json");

    return headers;
  }

  private HttpMessageErrorLogic(err:any){
    let textError = err.error;
    if(err.status==404){
      textError = "Узел не найден";
    }
    this.errorSub.next("Ошибка: " + textError);
  }
  public PostForm(apiUrl: string, qParams: HttpParams, Form: FormData): Observable<ApiAnswer|null> {
    let headers: HttpHeaders = new HttpHeaders();
    headers = headers.append("Content-type", "application/json");
    let post = new HttpRequest('POST',apiUrl,Form, {headers, params:qParams, withCredentials:true, responseType:"json"});
    return this.http.request<ApiAnswer>(post).pipe(map<HttpEvent<ApiAnswer>,ApiAnswer|null>(x=>{
      if(x.type == HttpEventType.Response){
        if(x.status!= 200 && x.body){
          this.errorSub.next(x.body?.message);
          return null;
        }else{
          return x.body;
        }

      }      else{
        return null;
      }
    }));

  }
  public Post(apiUrl: string, qParams: HttpParams, request: object): Observable<ApiAnswer|null> {
    let headers: HttpHeaders = new HttpHeaders();
    headers = headers.append("Content-type", "application/json");

    let body = JSON.stringify(request);

    let post = new HttpRequest('POST',apiUrl,body, {headers, params:qParams, withCredentials:true, responseType:"json"});
    return this.http.request<ApiAnswer>(post).pipe(map<HttpEvent<ApiAnswer>,ApiAnswer|null>(x=>{
      if(x.type == HttpEventType.Response){
        if(x.status!= 200 && x.body){
          this.errorSub.next(x.body?.message);
          return null;
        }else{
          return x.body;
        }
      }      else{
        return null;
      }
    }));

  }
  public Put(apiUrl: string, qParams: HttpParams, request: object): Observable<ApiAnswer|null> {
    let headers: HttpHeaders = new HttpHeaders();
    headers = headers.append("Content-type", "application/json");

    let body = JSON.stringify(request);

    let post = new HttpRequest('PUT',apiUrl,body, {headers, params:qParams, withCredentials:true, responseType:"json"});
    return this.http.request<ApiAnswer>(post).pipe(map<HttpEvent<ApiAnswer>,ApiAnswer|null>(x=>{
      if(x.type == HttpEventType.Response){
        if(x.status!= 200 && x.body){
          this.errorSub.next(x.body?.message);
          return null;
        }else{
          return x.body;
        }
      }      else{
        return null;
      }
    }));

  }

  public Get(apiUrl: string, qParams: HttpParams): Observable<ApiAnswer|null> {
    let headers: HttpHeaders = new HttpHeaders();
    headers = headers.append("Content-type", "application/json");
    let get = new HttpRequest('GET',apiUrl, {headers, params:qParams, withCredentials:true, responseType:"json"});
    return this.http.request<ApiAnswer>(get).pipe(map<HttpEvent<ApiAnswer>,ApiAnswer|null>(x=>{
      if(x.type == HttpEventType.Response){
        if(x.status!= 200 && x.body){
          this.errorSub.next(x.body?.message);
          return null;
        }else{
          return x.body;
        }
      }      else{
        return null;
      }
    }));
    /*
    return this.http.get( apiUrl, { headers, params: qParams, withCredentials:true, responseType:'json'})
      .pipe(timeout(this.timeout), catchError((err,caught) => {
        return new Observable<ApiAnswer>((subscriber => {
          subscriber.next({data:null,message:"Превышен интервал ожидания", result:false});
          subscriber.complete();
        }))}

      ), (map<object, ApiAnswer>((x) => {

        const answer: ApiAnswer =  <ApiAnswer> JSON.parse(x.toString());

        return answer;
      })));*/
  }

  public Upload(apiUrl:string,qParams:HttpParams,file:File, params:Array<{name:string,value:string}>):Observable<ApiAnswer|null>{
    let fd = new FormData();
    fd.append(file.name, file);
    params.forEach(x=>{
      fd.append(x.name,x.value);
    });
    let uploadReq = new HttpRequest('POST', apiUrl, fd, {
      reportProgress: false, withCredentials:true, params: new HttpParams(), responseType:'json'
    });
    return this.http.request(uploadReq).pipe(map<HttpEvent<any>,ApiAnswer|null>(x=>{
      if (x.type === HttpEventType.UploadProgress) {

      } else if (x.type == HttpEventType.Response) {
        return x.body;

      }
      return null;
    }));

  }

}
