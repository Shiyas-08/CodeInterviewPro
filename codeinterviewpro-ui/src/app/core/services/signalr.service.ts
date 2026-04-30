import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Subject } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class SignalrService {
  private hubConnection!: signalR.HubConnection;
  
  public codeChanged$ = new Subject<{ sessionId: string, code: string }>();
  public candidateStatusChanged$ = new Subject<boolean>();
  public timerUpdated$ = new Subject<number>();
  public interviewSubmitted$ = new Subject<void>();
  public interviewStopped$ = new Subject<void>();
  public pingCandidate$ = new Subject<void>();

  // WebRTC Subjects
  public webRTCOffer$ = new Subject<string>();
  public webRTCAnswer$ = new Subject<string>();
  public iceCandidate$ = new Subject<string>();

  constructor() {}

  public async startConnection(): Promise<void> {
    if (this.hubConnection && this.hubConnection.state === signalR.HubConnectionState.Connected) {
      return;
    }

    const hubUrl = environment.apiUrl.replace('/api', '') + '/interviewHub';

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(hubUrl, {
        transport: signalR.HttpTransportType.WebSockets | signalR.HttpTransportType.LongPolling
      })
      .withAutomaticReconnect()
      .build();

    try {
      await this.hubConnection.start();
      console.log('SignalR Connection started');
      this.addListeners();
    } catch (err) {
      console.log('Error while starting connection: ' + err);
    }
  }

  private addListeners() {
    this.hubConnection.on('CodeChanged', (sessionId: string, code: string) => {
      this.codeChanged$.next({ sessionId, code });
    });

    this.hubConnection.on('CandidateStatusChanged', (isOnline: boolean) => {
      this.candidateStatusChanged$.next(isOnline);
    });

    this.hubConnection.on('TimerUpdated', (remainingSeconds: number) => {
      this.timerUpdated$.next(remainingSeconds);
    });

    this.hubConnection.on('ReceiveWebRTCOffer', (offer: string) => {
      this.webRTCOffer$.next(offer);
    });

    this.hubConnection.on('ReceiveWebRTCAnswer', (answer: string) => {
      this.webRTCAnswer$.next(answer);
    });

    this.hubConnection.on('ReceiveIceCandidate', (candidate: string) => {
      this.iceCandidate$.next(candidate);
    });

    this.hubConnection.on('InterviewSubmitted', () => {
      this.interviewSubmitted$.next();
    });

    this.hubConnection.on('InterviewStopped', () => {
      console.log('[SignalR] InterviewStopped');
      this.interviewStopped$.next();
    });
    this.hubConnection.on('PingCandidate', () => {
      console.log('[SignalR] PingCandidate received');
      this.pingCandidate$.next();
    });
  }

  public joinCandidateRoom(sessionId: string) {
    if (!sessionId) return;
    const sid = sessionId.toLowerCase();
    console.log(`[SignalR] Joining Candidate Room: ${sid}`);
    if (this.hubConnection?.state === signalR.HubConnectionState.Connected) {
      this.hubConnection.invoke('JoinCandidateRoom', sid).catch(err => console.error(err));
    } else {
      setTimeout(() => this.joinCandidateRoom(sid), 500);
    }
  }

  public joinHRRoom(sessionId: string) {
    if (!sessionId) return;
    const sid = sessionId.toLowerCase();
    console.log(`[SignalR] Joining HR Room: ${sid}`);
    if (this.hubConnection?.state === signalR.HubConnectionState.Connected) {
      this.hubConnection.invoke('JoinHRRoom', sid).catch(err => console.error(err));
    } else {
      setTimeout(() => this.joinHRRoom(sid), 500);
    }
  }

  public sendCodeChanged(sessionId: string, code: string) {
    if (!sessionId) return;
    const sid = sessionId.toLowerCase();
    console.log(`[SignalR] Sending CodeChanged for ${sid}, length: ${code?.length}`);
    if (this.hubConnection?.state === signalR.HubConnectionState.Connected) {
      this.hubConnection.invoke('CodeChanged', sid, code).catch(err => console.error(err));
    }
  }

  public sendTimerUpdated(sessionId: string, remainingSeconds: number) {
    if (!sessionId) return;
    const sid = sessionId.toLowerCase();
    console.log(`[SignalR] Sending TimerUpdate: ${remainingSeconds} for ${sid}`);
    if (this.hubConnection?.state === signalR.HubConnectionState.Connected) {
      this.hubConnection.invoke('TimerUpdated', sid, remainingSeconds).catch(err => console.error(err));
    }
  }

  public sendCandidateStatusChanged(sessionId: string, isOnline: boolean) {
    if (!sessionId) return;
    const sid = sessionId.toLowerCase();
    console.log(`[SignalR] Sending Status Changed: ${isOnline} for ${sid}`);
    if (this.hubConnection?.state === signalR.HubConnectionState.Connected) {
      this.hubConnection.invoke('CandidateStatusChanged', sid, isOnline).catch(err => console.error(err));
    }
  }

  public sendInterviewSubmitted(sessionId: string) {
    if (this.hubConnection?.state === signalR.HubConnectionState.Connected) {
      this.hubConnection.invoke('InterviewSubmitted', sessionId).catch(err => console.error(err));
    }
  }

  // WebRTC Signals Emission
  public sendWebRTCOffer(sessionId: string, offer: string, isFromHR: boolean) {
    if (!sessionId) return;
    const sid = sessionId.toLowerCase();
    console.log(`[SignalR] Sending WebRTC Offer (from HR: ${isFromHR})`);
    if (this.hubConnection?.state === signalR.HubConnectionState.Connected) {
      this.hubConnection.invoke('SendWebRTCOffer', sid, offer, isFromHR).catch(err => console.error(err));
    }
  }

  public sendWebRTCAnswer(sessionId: string, answer: string, isFromHR: boolean) {
    if (!sessionId) return;
    const sid = sessionId.toLowerCase();
    console.log(`[SignalR] Sending WebRTC Answer (from HR: ${isFromHR})`);
    if (this.hubConnection?.state === signalR.HubConnectionState.Connected) {
      this.hubConnection.invoke('SendWebRTCAnswer', sid, answer, isFromHR).catch(err => console.error(err));
    }
  }

  public sendIceCandidate(sessionId: string, candidate: string, isFromHR: boolean) {
    if (!sessionId) return;
    const sid = sessionId.toLowerCase();
    if (this.hubConnection?.state === signalR.HubConnectionState.Connected) {
      this.hubConnection.invoke('SendIceCandidate', sid, candidate, isFromHR).catch(err => console.error(err));
    }
  }

  public sendPingCandidate(sessionId: string) {
    if (this.hubConnection?.state === signalR.HubConnectionState.Connected) {
      this.hubConnection.invoke('PingCandidate', sessionId).catch(err => console.error(err));
    }
  }
}
