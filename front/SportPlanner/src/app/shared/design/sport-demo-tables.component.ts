import { Component, computed, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProfessionalTableComponent, TableColumn, TableAction } from './professional-table.component';

interface Team {
	id: string;
	name: string;
	city: string;
	rating: number;
	wins: number;
	losses: number;
	members: string[];
	founded: string; // ISO date
}

@Component({
	selector: 'sp-sport-demo-tables',
	standalone: true,
	imports: [CommonModule, ProfessionalTableComponent],
		templateUrl: './sport-demo-tables.component.html',
})
export class SportDemoTablesComponent {
	view = signal<'table' | 'cards' | 'professional'>('professional');

	professionalColumns: TableColumn[] = [
		{ key: 'name', label: 'Equipo', sortable: true, filterable: true, type: 'text' },
		{ key: 'city', label: 'Ciudad', sortable: true, filterable: true, type: 'text' },
		{ key: 'rating', label: 'Rating', sortable: true, type: 'badge',
			cellClass: (row) => this.getStatus(row).text + ' ' + this.getStatus(row).color
		},
		{ key: 'members.length', label: 'Miembros', sortable: true, type: 'number' },
		{ key: 'wins', label: 'Victorias', sortable: true, type: 'number' },
		{ key: 'founded', label: 'Fundado', sortable: true, type: 'date' },
	];

	professionalActions: TableAction[] = [
		{ icon: 'fa-solid fa-eye', label: 'Ver', color: 'primary', onClick: (row) => alert(`Viendo ${row.name}`) },
		{ icon: 'fa-solid fa-pencil', label: 'Editar', color: 'secondary', onClick: (row) => alert(`Editando ${row.name}`) },
		{ icon: 'fa-solid fa-trash', label: 'Eliminar', color: 'danger', onClick: (row) => alert(`Eliminando ${row.name}`),
			show: (row) => row.rating < 7
		},
	];
	search = signal('');
		showFilters = signal(false);
		nameFilter = signal('');
		cityFilter = signal('');
		minRating = signal<number | null>(null);
		membersFilter = signal('');
	sortKey = signal<'rating' | 'name' | 'members' | 'wins'>('rating');
	sortDir = signal<'asc' | 'desc'>('desc');
	compact = signal(false);
		pageSize = signal<number>(10); // 10, 30, 0 means all
		pageIndex = signal<number>(0);

	teams = signal<Team[]>([
		{ id: 't1', name: 'Halcones RP', city: 'Madrid', rating: 8.7, wins: 12, losses: 3, members: ['Ana', 'Luis', 'Carlos', 'Marta'], founded: '2016-04-12' },
		{ id: 't2', name: 'Tiburones Norte', city: 'Barcelona', rating: 7.9, wins: 10, losses: 5, members: ['Pablo', 'Sofía', 'Diego'], founded: '2014-09-01' },
		{ id: 't3', name: 'Rayo Verde', city: 'Valencia', rating: 9.2, wins: 14, losses: 1, members: ['Lucía', 'Andrés', 'Óscar', 'María', 'Joan'], founded: '2018-06-20' },
		{ id: 't4', name: 'Cóndores', city: 'Sevilla', rating: 6.4, wins: 6, losses: 9, members: ['Iker', 'Noa'], founded: '2012-11-03' },
		{ id: 't5', name: 'Guerreros', city: 'Bilbao', rating: 7.1, wins: 8, losses: 7, members: ['Eneko', 'Ane', 'Jon'], founded: '2010-03-17' },
		{ id: 't6', name: 'Estrellas', city: 'Málaga', rating: 8.0, wins: 11, losses: 4, members: ['Carmen', 'Nuria', 'Raúl'], founded: '2019-01-08' },
		{ id: 't7', name: 'Leones Urbanos', city: 'Zaragoza', rating: 5.9, wins: 4, losses: 11, members: ['Hugo'], founded: '2008-07-22' },
		{ id: 't8', name: 'Águilas', city: 'Alicante', rating: 8.9, wins: 13, losses: 2, members: ['Rosa', 'Miguel', 'Sara', 'Lía'], founded: '2017-02-14' },
		{ id: 't9', name: 'Vikings', city: 'Pamplona', rating: 6.8, wins: 7, losses: 8, members: ['Iñigo', 'Amaia'], founded: '2013-12-30' },
		{ id: 't10', name: 'Dragones', city: 'Salamanca', rating: 9.5, wins: 15, losses: 0, members: ['Diego', 'Sergio', 'Paco', 'Laura', 'Nerea'], founded: '2020-05-05' },
	]);

	filteredTeams = computed(() => {
			const q = this.search().trim().toLowerCase();
			const n = this.nameFilter().trim().toLowerCase();
			const c = this.cityFilter().trim().toLowerCase();
			const m = this.membersFilter().trim().toLowerCase();
			const minR = this.minRating();

			let items = this.teams().filter(t => {
				if (q && !(t.name.toLowerCase().includes(q) || t.city.toLowerCase().includes(q))) return false;
				if (n && !t.name.toLowerCase().includes(n)) return false;
				if (c && !t.city.toLowerCase().includes(c)) return false;
				if (m && !t.members.join(' ').toLowerCase().includes(m)) return false;
				if (minR != null && t.rating < minR) return false;
				return true;
			});

		const key = this.sortKey();
		const dir = this.sortDir() === 'asc' ? 1 : -1;

		items = items.sort((a, b) => {
			if (key === 'name') return a.name.localeCompare(b.name) * dir;
			if (key === 'rating') return (a.rating - b.rating) * dir;
			if (key === 'members') return (a.members.length - b.members.length) * dir;
			if (key === 'wins') return (a.wins - b.wins) * dir;
			return 0;
		});

		return items;
	});

		pagedTeams = computed(() => {
			const all = this.filteredTeams();
			const size = this.pageSize();
			const idx = this.pageIndex();
			if (!size || size <= 0) return all;
			const start = idx * size;
			return all.slice(start, start + size);
		});

		totalPages = computed(() => {
			const size = this.pageSize();
			const total = this.filteredTeams().length;
			if (!size || size <= 0) return 1;
			return Math.max(1, Math.ceil(total / size));
		});

		goFirst() { this.pageIndex.set(0); }
		goPrev() { this.pageIndex.set(Math.max(0, this.pageIndex() - 1)); }
		goNext() { this.pageIndex.set(Math.min(this.totalPages() - 1, this.pageIndex() + 1)); }
		goLast() { this.pageIndex.set(Math.max(0, this.totalPages() - 1)); }

			goToPage(i: number) {
				if (i < 0) return;
				this.pageIndex.set(Math.min(Math.max(0, i), this.totalPages() - 1));
			}

			pages = computed(() => {
				const total = this.totalPages();
				const current = this.pageIndex();
				const maxButtons = 7;
				if (total <= maxButtons) return Array.from({ length: total }, (_, i) => i);

				const res: number[] = [];
				const left = Math.max(0, current - 2);
				const right = Math.min(total - 1, current + 2);

				if (left > 1) {
					res.push(0);
					res.push(-1); // ellipsis
				} else {
					for (let i = 0; i < left; i++) res.push(i);
				}

				for (let i = left; i <= right; i++) res.push(i);

				if (right < total - 2) {
					res.push(-1);
					res.push(total - 1);
				} else {
					for (let i = right + 1; i < total; i++) res.push(i);
				}

				return res;
			});

			setPageSize(v: string | number) {
				const n = typeof v === 'number' ? v : parseInt(String(v), 10);
				this.pageSize.set(isNaN(n) ? 0 : n);
				this.pageIndex.set(0);
			}

	toggleSort(key: 'rating' | 'name' | 'members' | 'wins') {
		if (this.sortKey() === key) {
			this.sortDir.set(this.sortDir() === 'asc' ? 'desc' : 'asc');
		} else {
			this.sortKey.set(key);
			this.sortDir.set('desc');
		}
	}

	getRatingPercent(t: Team) {
		const pct = (t.rating / 10) * 100;
		return Math.min(100, Math.max(0, Math.round(pct)));
	}

		getStatus(t: Team) {
			if (t.rating >= 9) return { label: 'Elite', color: 'from-yellow-400 to-orange-500', text: 'amber-800' };
			if (t.rating >= 8) return { label: 'Contender', color: 'from-emerald-400 to-teal-500', text: 'emerald-900' };
			if (t.rating >= 7) return { label: 'Stable', color: 'from-sky-300 to-indigo-400', text: 'sky-900' };
			return { label: 'Rebuild', color: 'from-slate-300 to-slate-400', text: 'slate-800' };
		}

		formatDate(iso: string) {
			try {
				const d = new Date(iso);
				return d.toLocaleDateString();
			} catch {
				return iso;
			}
		}

		exportCSV() {
			const rows = this.filteredTeams().map(t => ({
				id: t.id,
				name: t.name,
				city: t.city,
				rating: t.rating,
				wins: t.wins,
				losses: t.losses,
				members: t.members.join('|'),
				founded: t.founded,
			}));

			const header = Object.keys(rows[0] || {}).join(',');
			const lines = rows.map(r => Object.values(r).map(v => `"${(String(v || '')).replace(/"/g, '""')}"`).join(','));
			const csv = [header, ...lines].join('\n');

			const blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' });
			const url = URL.createObjectURL(blob);
			const a = document.createElement('a');
			a.href = url;
			a.download = 'teams-export.csv';
			a.click();
			URL.revokeObjectURL(url);
		}
}

