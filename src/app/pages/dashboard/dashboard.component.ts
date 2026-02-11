import { Component } from '@angular/core';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [],
  template: `
    <div class="dashboard-container">
      <div class="hero-section">
        <div class="hero-content">
          <h1>Smart Appointment Booking System</h1>
          <p class="hero-subtitle">Schedule your appointments with ease and confidence</p>
          <div class="hero-actions">
            <a href="/available-slots" class="btn btn-primary">Book Appointment</a>
            <a href="/my-appointments" class="btn btn-secondary">My Appointments</a>
          </div>
        </div>
      </div>

      <div class="features-section">
        <h2>Why Choose Our System?</h2>
        <div class="features-grid">
          <div class="feature-card">
            <div class="feature-icon">📅</div>
            <h3>View Available Slots</h3>
            <p>See all available time slots in real-time</p>
          </div>
          <div class="feature-card">
            <div class="feature-icon">✅</div>
            <h3>Easy Booking</h3>
            <p>Book appointments in just a few clicks</p>
          </div>
          <div class="feature-card">
            <div class="feature-icon">🚫</div>
            <h3>No Double Booking</h3>
            <p>Automatic conflict detection prevents overlaps</p>
          </div>
          <div class="feature-card">
            <div class="feature-icon">⚙️</div>
            <h3>Admin Management</h3>
            <p>Full control over slots and appointments</p>
          </div>
        </div>
      </div>

      <div class="stats-section">
        <div class="stat-card">
          <div class="stat-number">24/7</div>
          <div class="stat-label">Online Booking</div>
        </div>
        <div class="stat-card">
          <div class="stat-number">100%</div>
          <div class="stat-label">Conflict Prevention</div>
        </div>
        <div class="stat-card">
          <div class="stat-number">Instant</div>
          <div class="stat-label">Confirmation</div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .dashboard-container {
      min-height: 100vh;
    }

    .hero-section {
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      color: white;
      padding: 80px 20px;
      text-align: center;
    }

    .hero-content h1 {
      font-size: 3rem;
      font-weight: 700;
      margin-bottom: 1rem;
      animation: fadeInUp 0.8s ease-out;
    }

    .hero-subtitle {
      font-size: 1.5rem;
      margin-bottom: 2rem;
      opacity: 0.9;
      animation: fadeInUp 0.8s ease-out 0.2s both;
    }

    .hero-actions {
      display: flex;
      gap: 1rem;
      justify-content: center;
      animation: fadeInUp 0.8s ease-out 0.4s both;
    }

    .btn {
      padding: 12px 32px;
      border-radius: 8px;
      text-decoration: none;
      font-weight: 600;
      transition: all 0.3s ease;
      display: inline-block;
    }

    .btn-primary {
      background: white;
      color: #667eea;
    }

    .btn-primary:hover {
      transform: translateY(-2px);
      box-shadow: 0 10px 30px rgba(0,0,0,0.2);
    }

    .btn-secondary {
      background: transparent;
      color: white;
      border: 2px solid white;
    }

    .btn-secondary:hover {
      background: white;
      color: #667eea;
      transform: translateY(-2px);
    }

    .features-section {
      padding: 80px 20px;
      max-width: 1200px;
      margin: 0 auto;
    }

    .features-section h2 {
      text-align: center;
      font-size: 2.5rem;
      margin-bottom: 3rem;
      color: #2d3748;
    }

    .features-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
      gap: 2rem;
    }

    .feature-card {
      background: white;
      padding: 2rem;
      border-radius: 12px;
      box-shadow: 0 4px 20px rgba(0,0,0,0.08);
      text-align: center;
      transition: all 0.3s ease;
    }

    .feature-card:hover {
      transform: translateY(-10px);
      box-shadow: 0 10px 40px rgba(0,0,0,0.15);
    }

    .feature-icon {
      font-size: 3rem;
      margin-bottom: 1rem;
    }

    .feature-card h3 {
      font-size: 1.5rem;
      color: #2d3748;
      margin-bottom: 0.5rem;
    }

    .feature-card p {
      color: #718096;
      line-height: 1.6;
    }

    .stats-section {
      background: #f7fafc;
      padding: 60px 20px;
      display: flex;
      justify-content: center;
      gap: 4rem;
      flex-wrap: wrap;
    }

    .stat-card {
      text-align: center;
    }

    .stat-number {
      font-size: 3rem;
      font-weight: 700;
      color: #667eea;
      margin-bottom: 0.5rem;
    }

    .stat-label {
      font-size: 1.1rem;
      color: #4a5568;
      font-weight: 600;
    }

    @keyframes fadeInUp {
      from {
        opacity: 0;
        transform: translateY(30px);
      }
      to {
        opacity: 1;
        transform: translateY(0);
      }
    }

    @media (max-width: 768px) {
      .hero-content h1 {
        font-size: 2rem;
      }

      .hero-subtitle {
        font-size: 1.1rem;
      }

      .hero-actions {
        flex-direction: column;
        align-items: center;
      }

      .btn {
        width: 200px;
      }

      .stats-section {
        gap: 2rem;
      }
    }
  `]
})
export class DashboardComponent {}
