import * as signalR from '@microsoft/signalr';
import { NotificationItem } from '../types';

let connection: signalR.HubConnection | null = null;

export const startSignalRConnection = (
  token: string,
  onNotificationReceived: (notification: NotificationItem) => void
) => {
  if (connection && connection.state === signalR.HubConnectionState.Connected) {
    return connection;
  }

  connection = new signalR.HubConnectionBuilder()
    .withUrl('/hubs/notifications', {
      accessTokenFactory: () => token,
      skipNegotiation: false,
    })
    .withAutomaticReconnect()
    .build();

  connection.on('ReceiveNotification', (data: NotificationItem) => {
    onNotificationReceived(data);
  });

  connection
    .start()
    .then(() => {
      console.log('[SignalR] Connected to Notification Hub');
    })
    .catch((err) => {
      console.warn('[SignalR] Real-time hub connection notice:', err);
    });

  return connection;
};

export const stopSignalRConnection = () => {
  if (connection) {
    connection.stop();
    connection = null;
  }
};
