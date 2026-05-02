# ChatApp Real-Time Communication

This section documents the real-time features and WebSocket architecture.

## WebSocket Architecture (Planned)

### 1. **Connection Flow**
```
Client
   ↓ WebSocket connect
API Gateway
   ↓ Upgrade to WebSocket
SignalR Hub (ChatHub)
   ↓ Authenticate user
Store connection in ConnectionManager
   ↓ Subscribe to user groups
   ↓ Subscribe to conversation groups
```

### 2. **SignalR Hubs** (Using .NET SignalR)

```csharp
public class ChatHub : Hub
{
    // Join conversation room
    public async Task JoinConversation(Guid conversationId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"conv_{conversationId}");
    }
    
    // Receive message from client
    public async Task SendMessage(Guid conversationId, string content)
    {
        // Validate
        // Save to DB
        // Broadcast to group
        await Clients.Group($"conv_{conversationId}").SendAsync("ReceiveMessage", message);
    }
    
    // User typing indicator
    public async Task StartTyping(Guid conversationId, string username)
    {
        await Clients.GroupExcept($"conv_{conversationId}", Context.ConnectionId)
            .SendAsync("UserTyping", username);
    }
    
    // Presence update
    public async Task UpdatePresence(OnlineStatus status)
    {
        // Update user status in DB
        // Broadcast to all connected users
    }
}
```

### 3. **Real-Time Events**

#### Message Events
- `message.sent` - New message created
- `message.edited` - Message edited
- `message.deleted` - Message deleted
- `message.reaction.added` - Reaction added
- `message.reaction.removed` - Reaction removed

#### Presence Events
- `user.online` - User came online
- `user.offline` - User went offline
- `user.away` - User set away
- `user.typing` - User typing indicator
- `user.stopped.typing` - User stopped typing

#### Conversation Events
- `conversation.created` - New conversation
- `conversation.member.joined` - User joined
- `conversation.member.left` - User left
- `conversation.member.banned` - User banned
- `conversation.updated` - Settings changed

#### Notification Events
- `notification.received` - New notification

### 4. **Typing Indicators**

```javascript
// Client
let typingTimer = null;

textarea.addEventListener('input', () => {
    clearTimeout(typingTimer);
    
    // Send typing event
    connection.invoke("StartTyping", conversationId, username);
    
    // Clear after 1 second of inactivity
    typingTimer = setTimeout(() => {
        connection.invoke("StopTyping", conversationId);
    }, 1000);
});
```

### 5. **Connection Management**

```csharp
public class ConnectionManager
{
    private readonly Dictionary<string, string> _userConnections = new();
    
    public void AddConnection(string userId, string connectionId)
    {
        _userConnections[connectionId] = userId;
    }
    
    public void RemoveConnection(string connectionId)
    {
        _userConnections.Remove(connectionId);
    }
    
    public List<string> GetUserConnections(string userId)
    {
        return _userConnections
            .Where(x => x.Value == userId)
            .Select(x => x.Key)
            .ToList();
    }
    
    // Broadcast to all user's connections
    public async Task BroadcastToUser(
        string userId, 
        string method, 
        object message,
        IHubContext<ChatHub> hub)
    {
        var connections = GetUserConnections(userId);
        foreach (var connectionId in connections)
        {
            await hub.Clients.Client(connectionId).SendAsync(method, message);
        }
    }
}
```

---

## Presence System

### 1. **Presence Tracking**

```csharp
public class PresenceService
{
    public async Task UpdatePresence(Guid userId, OnlineStatus status)
    {
        var user = await _repo.GetUser(userId);
        user.OnlineStatus = status;
        user.LastActiveAt = DateTime.UtcNow;
        
        await _repo.SaveChanges();
        
        // Broadcast to contacts
        await _hub.Clients.Group($"contacts_{userId}")
            .SendAsync("UserPresenceChanged", userId, status);
    }
    
    public async Task HandleDisconnect(Guid userId)
    {
        var user = await _repo.GetUser(userId);
        user.OnlineStatus = OnlineStatus.Offline;
        user.LastActiveAt = DateTime.UtcNow;
        
        await _repo.SaveChanges();
    }
}
```

### 2. **Idle Detection**
```
- Monitor last activity
- After 5 minutes of inactivity → Set status to "Away"
- After 30 minutes of inactivity → Set status to "Offline"
- Configurable per user preferences
```

### 3. **Multi-Device Handling**
```
If user is online on Mobile + Desktop:
- Show "Online" if ANY device is active
- Show device indicator (mobile/web icon)
- Can disable notifications on one device
```

---

## Message Delivery Guarantees

### 1. **Delivery Status Flow**
```
Sent
  ↓ (Message created in DB)
Delivered
  ↓ (Reached recipient's connected client)
Read
  ↓ (User has seen message - 2 sec visibility)
```

### 2. **Offline Message Queue**

```csharp
public class OfflineMessageQueue
{
    public async Task<List<Message>> GetPendingMessages(Guid userId)
    {
        // Get messages since last active time
        return await _repo.GetMessages(
            userId,
            sincce: user.LastActiveAt
        );
    }
    
    public async Task DeliverPendingMessages(Guid userId)
    {
        var messages = await GetPendingMessages(userId);
        
        foreach (var msg in messages)
        {
            await _hub.Clients.User(userId.ToString())
                .SendAsync("ReceiveMessage", msg);
        }
    }
}
```

### 3. **Retry Strategy**
- If delivery fails: Retry with exponential backoff
- Max 5 retries over 1 hour
- After 1 hour: Store in offline queue
- When user comes online: Deliver from queue

---

## Call System (Planned)

### 1. **Call Signaling**
```
A → "Initiate Call" → B
B → "Ringing" → A
B → "Accept Call" → A
A ↔ B (Exchange WebRTC Offers/Answers)
A ↔ B (Direct P2P Media Stream)
```

### 2. **WebRTC Integration**
```javascript
// Client setup
const peerConnection = new RTCPeerConnection({
    iceServers: [
        { urls: 'stun:stun.l.google.com:19302' },
        { urls: 'turn:your-turn-server.com', username: '...', credential: '...' }
    ]
});

// Share camera
const stream = await navigator.mediaDevices.getUserMedia({ 
    audio: true, 
    video: { width: 1280, height: 720 } 
});

stream.getTracks().forEach(track => 
    peerConnection.addTrack(track, stream)
);
```

### 3. **Notification System**
```
Incoming Call:
- Ring tone (optional)
- Desktop notification
- Mobile push notification
- Visual notification in app

Missed Call:
- Notification in inbox
- Add to recent calls
- Can view caller details
```

---

## Notification Delivery

### 1. **Notification Channels**
```
In-App
  ↓ (WebSocket real-time)
  ↓ (Immediate, if online)

Push Notification (Mobile)
  ↓ (Firebase Cloud Messaging)
  ↓ (If app is background/closed)

Email Notification
  ↓ (Digest, if configured)
  ↓ (Daily or weekly digest)
```

### 2. **Notification Preferences**
```csharp
public class NotificationPreference
{
    public Guid UserId { get; set; }
    
    public bool ReceiveMessageNotifications { get; set; } = true;
    public bool ReceiveCallNotifications { get; set; } = true;
    public bool ReceiveMentionNotifications { get; set; } = true;
    
    public NotificationDelivery DeliveryMethod { get; set; }  // InApp, Push, Email
    public bool DoNotDisturbEnabled { get; set; } = false;
    public TimeSpan? DoNotDisturbStart { get; set; }      // HH:mm
    public TimeSpan? DoNotDisturbEnd { get; set; }        // HH:mm
}
```

### 3. **Rate Limiting**
```
Per User, Per Hour:
- Max 20 in-app notifications
- Max 5 push notifications
- Exceptions: Mentions, Calls
```

---

## Scalability Considerations

### 1. **Load Balancing**
```
Multiple SignalR servers behind load balancer
  ↓ Use Redis backplane for communication
  ↓ Groups work across servers
  ↓ Sticky sessions for WebSocket connections
```

### 2. **Redis Backplane Configuration**
```csharp
services.AddSignalR()
    .AddStackExchangeRedis(options =>
    {
        options.ConnectionFactory = async writer =>
        {
            var connection = await ConnectionMultiplexer
                .ConnectAsync("redis-server:6379");
            return connection;
        };
    });
```

### 3. **Database Query Optimization**
```
For real-time message delivery:
- Cache recent messages in Redis
- Paginate message history (50 at a time)
- Use read replicas for analytics
```

---

## Security in Real-Time

### 1. **WebSocket Authentication**
```csharp
services.AddSignalR()
    .AddAzureSignalR();

public override async Task OnConnectedAsync()
{
    var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    
    if (string.IsNullOrEmpty(userId))
    {
        throw new HubException("Unauthorized");
    }
    
    await Clients.All.SendAsync("UserConnected", userId);
    await base.OnConnectedAsync();
}
```

### 2. **Message Validation**
```csharp
public async Task SendMessage(Guid conversationId, string content)
{
    // Validate user is member
    // Validate not banned
    // Validate content length
    // Sanitize HTML
    // Check rate limit
    
    var message = new Message { /* ... */ };
    await _repo.Add(message);
    
    // Broadcast
}
```

### 3. **Connection Cleanup**
```csharp
public override async Task OnDisconnectedAsync(Exception? exception)
{
    var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    _connectionManager.RemoveConnection(Context.ConnectionId);
    
    // Update user presence
    await _presenceService.HandleDisconnect(Guid.Parse(userId));
    
    await base.OnDisconnectedAsync(exception);
}
```

---

## Monitoring & Metrics

### 1. **Key Metrics**
- Active connections per hub
- Message delivery latency
- Failed deliveries
- Connection churn rate
- Peak concurrent users

### 2. **Logging**
```csharp
public void Configure(IHubContext<ChatHub> hub)
{
    hub.TraceWriter = new DiagnosticsTraceWriter();
}
```

### 3. **Alerting**
- Alert if message delivery > 500ms
- Alert if >20% failed deliveries
- Alert if connection loss spike
