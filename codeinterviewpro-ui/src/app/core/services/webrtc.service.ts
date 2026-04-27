import { Injectable } from '@angular/core';
import { Subject, ReplaySubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class WebrtcService {
  public peerConnection!: RTCPeerConnection;
  public localStream!: MediaStream;
  public remoteStream$ = new ReplaySubject<MediaStream>(1);
  
  public isAudioMuted = false;
  public isVideoMuted = false;

  private iceCandidatesBuffer: RTCIceCandidateInit[] = [];

  private configuration: RTCConfiguration = {
    iceServers: [
      { urls: 'stun:stun.l.google.com:19302' },
      { urls: 'stun:stun1.l.google.com:19302' },
      { urls: 'stun:stun2.l.google.com:19302' }
    ]
  };

  constructor() { }

  public async initLocalStream(): Promise<MediaStream> {
    try {
      this.localStream = await navigator.mediaDevices.getUserMedia({
        video: true,
        audio: true
      });
      return this.localStream;
    } catch (error) {
      console.error('Error accessing media devices.', error);
      throw error;
    }
  }

  public createPeerConnection(
    onIceCandidate: (candidate: RTCIceCandidate) => void
  ): RTCPeerConnection {
    if (this.peerConnection) {
      this.peerConnection.close();
    }
    this.peerConnection = new RTCPeerConnection(this.configuration);
    this.iceCandidatesBuffer = [];

    this.peerConnection.onicecandidate = (event) => {
      if (event.candidate) {
        onIceCandidate(event.candidate);
      }
    };

    this.peerConnection.ontrack = (event) => {
      console.log('Remote track received:', event);
      if (event.streams && event.streams[0]) {
        this.remoteStream$.next(event.streams[0]);
      }
    };

    if (this.localStream) {
      this.localStream.getTracks().forEach(track => {
        this.peerConnection.addTrack(track, this.localStream);
      });
    }

    return this.peerConnection;
  }

  public async createOffer(): Promise<RTCSessionDescriptionInit> {
    const offer = await this.peerConnection.createOffer({
      offerToReceiveAudio: true,
      offerToReceiveVideo: true
    });
    await this.peerConnection.setLocalDescription(offer);
    return offer;
  }

  public async handleOffer(offer: RTCSessionDescriptionInit): Promise<RTCSessionDescriptionInit> {
    await this.peerConnection.setRemoteDescription(new RTCSessionDescription(offer));
    await this.processBufferedIceCandidates();
    const answer = await this.peerConnection.createAnswer();
    await this.peerConnection.setLocalDescription(answer);
    return answer;
  }

  public async handleAnswer(answer: RTCSessionDescriptionInit): Promise<void> {
    await this.peerConnection.setRemoteDescription(new RTCSessionDescription(answer));
    await this.processBufferedIceCandidates();
  }

  public async handleIceCandidate(candidate: RTCIceCandidateInit): Promise<void> {
    if (this.peerConnection?.remoteDescription) {
      try {
        await this.peerConnection.addIceCandidate(new RTCIceCandidate(candidate));
      } catch (e) {
        console.error('Error adding received ice candidate', e);
      }
    } else {
      console.log('Buffering ICE candidate as remote description is not set yet');
      this.iceCandidatesBuffer.push(candidate);
    }
  }

  private async processBufferedIceCandidates() {
    console.log(`Processing ${this.iceCandidatesBuffer.length} buffered ICE candidates`);
    while (this.iceCandidatesBuffer.length > 0) {
      const candidate = this.iceCandidatesBuffer.shift();
      if (candidate) {
        try {
          await this.peerConnection.addIceCandidate(new RTCIceCandidate(candidate));
        } catch (e) {
          console.error('Error adding buffered ice candidate', e);
        }
      }
    }
  }

  public toggleAudio() {
    this.isAudioMuted = !this.isAudioMuted;
    console.log('Audio muted:', this.isAudioMuted);
    if (this.localStream) {
      this.localStream.getAudioTracks().forEach(t => t.enabled = !this.isAudioMuted);
    }
  }

  public toggleVideo() {
    this.isVideoMuted = !this.isVideoMuted;
    console.log('Video muted:', this.isVideoMuted);
    if (this.localStream) {
      this.localStream.getVideoTracks().forEach(t => t.enabled = !this.isVideoMuted);
    }
  }

  public endCall() {
    if (this.peerConnection) {
      this.peerConnection.close();
    }
    if (this.localStream) {
      this.localStream.getTracks().forEach(track => track.stop());
    }
    this.isAudioMuted = false;
    this.isVideoMuted = false;
    this.iceCandidatesBuffer = [];
  }
}
