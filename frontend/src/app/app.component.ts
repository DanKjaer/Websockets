import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import {FormControl, ReactiveFormsModule} from "@angular/forms";
import ReconnectingWebSocket from "reconnecting-websocket";
import {BaseDto, ServerBroadcastsClientDto, ServerConnectsToClientDto, ServerEchosClientDto} from "../BaseDto";

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, ReactiveFormsModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'frontend';

  messages: string[] = [];

  ws: WebSocket = new WebSocket("ws://localhost:8181");
  rws: ReconnectingWebSocket = new ReconnectingWebSocket("ws://localhost:8181");

  messageContent =
    new FormControl('');

  constructor() {
    this.rws.onopen = () => {
      this.newConnection();
    }
    this.rws.onmessage = message => {
      const messageFromServer = JSON.parse(message.data) as BaseDto<any>
      // @ts-ignore
      this[messageFromServer.eventType].call(this, messageFromServer);
    }
  }

  ServerEchosClient(dto: ServerEchosClientDto) {
    this.messages.push(dto.echoValue!);
  }

  ServerBroadcastsClient(dto: ServerBroadcastsClientDto) {
    this.messages.push(dto.broadcastValue!);
  }

  ServerConnectsToClient(dto: ServerConnectsToClientDto) {
    this.messages.push(dto.welcomeValue!, dto.notificationValue!);
  }

  newConnection() {
    var object = {
      eventType: "ClientConnectsToServer",
      welcomeMessage: "",
      notificationMessage: ""
    }
    this.rws.send(JSON.stringify(object));
  }

  sendMessage() {
    var object = {
      eventType: "ClientWantsToEchoServer",
      messageContent: this.messageContent.value!
    }
    var object2 = {
      eventType: "ClientWantsToBroadcastToServer",
      messageContent: this.messageContent.value!
    }
    this.rws.send(JSON.stringify(object));
    this.rws.send(JSON.stringify(object2));
  }
}
