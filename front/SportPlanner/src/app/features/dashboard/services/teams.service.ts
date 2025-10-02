export class TeamsService {
  private base = '/api/subscriptions';

  // Helper to detect browser vs SSR
  private isBrowser() {
    return typeof window !== 'undefined' && !!window?.fetch;
  }

  async getTeams(subscriptionId: string) {
    if (!this.isBrowser()) {
      // During SSR return empty list to avoid server fetch of relative URLs
      return [] as any[];
    }
    const res = await fetch(`${this.base}/${subscriptionId}/teams`, {
      headers: { 'Accept': 'application/json' }
    });
    if (!res.ok) throw new Error(`Failed to fetch teams: ${res.status}`);
    return await res.json();
  }

  async getTeam(subscriptionId: string, teamId: string) {
    if (!this.isBrowser()) return null;
    const res = await fetch(`${this.base}/${subscriptionId}/teams/${teamId}`, {
      headers: { 'Accept': 'application/json' }
    });
    if (!res.ok) throw new Error(`Failed to fetch team: ${res.status}`);
    return await res.json();
  }

  async createTeam(subscriptionId: string, payload: any) {
    if (!this.isBrowser()) return null;
    const res = await fetch(`${this.base}/${subscriptionId}/teams`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(payload)
    });
    if (!res.ok) throw new Error(`Failed to create team: ${res.status}`);
    return await res.json();
  }

  async updateTeam(subscriptionId: string, teamId: string, payload: any) {
    if (!this.isBrowser()) return null;
    const res = await fetch(`${this.base}/${subscriptionId}/teams/${teamId}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(payload)
    });
    if (!res.ok) throw new Error(`Failed to update team: ${res.status}`);
    return await res.json();
  }

  async deleteTeam(subscriptionId: string, teamId: string) {
    if (!this.isBrowser()) return false;
    const res = await fetch(`${this.base}/${subscriptionId}/teams/${teamId}`, {
      method: 'DELETE'
    });
    return res.ok;
  }
}
