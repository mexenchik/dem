"""
ООО «Лёгкий Шаг» — Демонстрационный экзамен
Стек: Python + tkinter + SQLite (нулевые зависимости)
Роли: Guest → Client → Manager → Admin
"""

import tkinter as tk
from tkinter import ttk, messagebox
import sqlite3
import hashlib
import datetime

# ================================================================
#  КОНСТАНТЫ (ЦВЕТА И ШРИФТЫ ПО ТЗ)
# ================================================================
BG_MAIN   = "#FFFFFF"   # основной фон
BG_SEC    = "#7FFF00"   # дополнительный фон (заголовки, панели)
BG_ACCENT = "#00FA9A"   # акцент (кнопки целевого действия)
BG_DISC   = "#2E8B57"   # скидка > 15%
FG_DISC   = "#FFFFFF"

FONT      = ("Times New Roman", 11)
FONT_BOLD = ("Times New Roman", 11, "bold")
FONT_H    = ("Times New Roman", 14, "bold")

DB_PATH = "legky_shag.db"

# ================================================================
#  БАЗА ДАННЫХ
# ================================================================
def get_conn():
    conn = sqlite3.connect(DB_PATH)
    conn.row_factory = sqlite3.Row
    return conn

def init_db():
    conn = get_conn()
    c = conn.cursor()
    c.executescript("""
        CREATE TABLE IF NOT EXISTS roles (
            id        INTEGER PRIMARY KEY,
            role_name TEXT NOT NULL
        );
        CREATE TABLE IF NOT EXISTS users (
            id            INTEGER PRIMARY KEY AUTOINCREMENT,
            login         TEXT NOT NULL UNIQUE,
            password_hash TEXT NOT NULL,
            role_id       INTEGER NOT NULL,
            FOREIGN KEY (role_id) REFERENCES roles(id)
        );
        CREATE TABLE IF NOT EXISTS products (
            id       INTEGER PRIMARY KEY AUTOINCREMENT,
            name     TEXT    NOT NULL,
            article  TEXT    NOT NULL UNIQUE,
            price    REAL    NOT NULL,
            size     REAL    NOT NULL,
            brand    TEXT    NOT NULL,
            quantity INTEGER NOT NULL,
            discount REAL    DEFAULT 0
        );
        CREATE TABLE IF NOT EXISTS orders (
            id           INTEGER PRIMARY KEY AUTOINCREMENT,
            user_id      INTEGER,
            date         TEXT NOT NULL,
            status       TEXT NOT NULL,
            total_amount REAL NOT NULL,
            FOREIGN KEY (user_id) REFERENCES users(id)
        );
        CREATE TABLE IF NOT EXISTS order_items (
            id              INTEGER PRIMARY KEY AUTOINCREMENT,
            order_id        INTEGER NOT NULL,
            product_id      INTEGER NOT NULL,
            count           INTEGER NOT NULL,
            price_at_moment REAL    NOT NULL,
            FOREIGN KEY (order_id)   REFERENCES orders(id),
            FOREIGN KEY (product_id) REFERENCES products(id)
        );
    """)

    # Роли (Гость в БД не хранится — это режим приложения)
    if c.execute("SELECT COUNT(*) FROM roles").fetchone()[0] == 0:
        c.executemany("INSERT INTO roles(id, role_name) VALUES(?,?)", [
            (1, "Client"),
            (2, "Manager"),
            (3, "Admin"),
        ])

    # Тестовые пользователи
    if c.execute("SELECT COUNT(*) FROM users").fetchone()[0] == 0:
        def h(p): return hashlib.sha256(p.encode()).hexdigest()
        c.executemany("INSERT INTO users(login, password_hash, role_id) VALUES(?,?,?)", [
            ("client",  h("client123"),  1),
            ("manager", h("manager123"), 2),
            ("admin",   h("admin123"),   3),
        ])

    # Тестовые товары
    if c.execute("SELECT COUNT(*) FROM products").fetchone()[0] == 0:
        c.executemany(
            "INSERT INTO products(name,article,price,size,brand,quantity,discount) VALUES(?,?,?,?,?,?,?)",
            [
                ("Кроссовки Air Max",  "AM-001", 8500.0,  42, "Nike",       15,  0.0),
                ("Туфли классика",     "TK-002", 5200.0,  40, "Ecco",        8, 10.0),
                ("Ботинки зимние",     "BZ-003", 12000.0, 43, "Timberland",  5, 20.0),
                ("Сандалии летние",    "SL-004", 2800.0,  38, "Crocs",      20,  5.0),
                ("Кеды повседневные",  "KP-005", 3500.0,  41, "Converse",   12, 16.0),
                ("Мокасины замшевые",  "MZ-006", 4100.0,  39, "Clarks",      7,  0.0),
            ]
        )

    # Тестовые заказы
    if c.execute("SELECT COUNT(*) FROM orders").fetchone()[0] == 0:
        c.executemany(
            "INSERT INTO orders(user_id,date,status,total_amount) VALUES(?,?,?,?)",
            [
                (1, "2026-05-01", "Доставлен",    8500.0),
                (1, "2026-05-15", "В обработке",  5200.0),
                (2, "2026-05-20", "Новый",        15200.0),
            ]
        )

    conn.commit()
    conn.close()

def hash_password(pwd: str) -> str:
    return hashlib.sha256(pwd.encode()).hexdigest()

def check_login(login: str, password: str):
    """Возвращает (id, login, role_name) или None."""
    conn = get_conn()
    row = conn.execute("""
        SELECT u.id, u.login, r.role_name
        FROM users u
        JOIN roles r ON u.role_id = r.id
        WHERE u.login = ? AND u.password_hash = ?
    """, (login, hash_password(password))).fetchone()
    conn.close()
    return row

# ================================================================
#  ГЛОБАЛЬНОЕ СОСТОЯНИЕ ПОЛЬЗОВАТЕЛЯ
# ================================================================
current_user = {"id": None, "login": "Гость", "role": "Guest"}

# ================================================================
#  ВСПОМОГАТЕЛЬНАЯ ФУНКЦИЯ — кнопка с цветом акцента
# ================================================================
def accent_btn(parent, text, command, color=BG_ACCENT, **kw):
    return tk.Button(parent, text=text, command=command,
                     font=FONT, bg=color, activebackground=color,
                     relief="flat", padx=8, pady=4, **kw)

# ================================================================
#  ФОРМА АВТОРИЗАЦИИ
# ================================================================
class LoginForm(tk.Tk):
    def __init__(self):
        super().__init__()
        self.title("Лёгкий Шаг — Вход в систему")
        self.configure(bg=BG_MAIN)
        self.resizable(False, False)
        self._center(400, 290)

        # Шапка
        tk.Label(self, text="ООО «Лёгкий Шаг»",
                 font=FONT_H, bg=BG_SEC, pady=10).pack(fill="x")

        # Форма
        body = tk.Frame(self, bg=BG_MAIN, padx=40, pady=15)
        body.pack(fill="both", expand=True)

        tk.Label(body, text="Логин:", font=FONT, bg=BG_MAIN, anchor="w").grid(
            row=0, column=0, sticky="w", pady=6)
        self._login = tk.StringVar()
        tk.Entry(body, textvariable=self._login, font=FONT, width=22).grid(
            row=0, column=1, pady=6, padx=(10, 0))

        tk.Label(body, text="Пароль:", font=FONT, bg=BG_MAIN, anchor="w").grid(
            row=1, column=0, sticky="w", pady=6)
        self._pwd = tk.StringVar()
        tk.Entry(body, textvariable=self._pwd, show="*", font=FONT, width=22).grid(
            row=1, column=1, pady=6, padx=(10, 0))

        self._err = tk.Label(body, text="", fg="red", bg=BG_MAIN, font=FONT)
        self._err.grid(row=2, column=0, columnspan=2, pady=(0, 4))

        accent_btn(body, "Войти", self._do_login, width=18).grid(
            row=3, column=0, columnspan=2, pady=4)
        accent_btn(body, "Войти как Гость", self._do_guest,
                   color=BG_SEC, width=18).grid(row=4, column=0, columnspan=2, pady=2)

        # Enter → войти
        self.bind("<Return>", lambda e: self._do_login())

    def _center(self, w, h):
        sw, sh = self.winfo_screenwidth(), self.winfo_screenheight()
        self.geometry(f"{w}x{h}+{(sw-w)//2}+{(sh-h)//2}")

    def _do_login(self):
        login = self._login.get().strip()
        pwd   = self._pwd.get().strip()
        if not login or not pwd:
            self._err.config(text="Введите логин и пароль")
            return
        row = check_login(login, pwd)
        if row:
            current_user["id"]    = row["id"]
            current_user["login"] = row["login"]
            current_user["role"]  = row["role_name"]
            self._open_main()
        else:
            self._err.config(text="Неверный логин или пароль")

    def _do_guest(self):
        current_user["id"]    = None
        current_user["login"] = "Гость"
        current_user["role"]  = "Guest"
        self._open_main()

    def _open_main(self):
        self.destroy()
        MainWindow().mainloop()

# ================================================================
#  ГЛАВНОЕ ОКНО
# ================================================================
class MainWindow(tk.Tk):
    def __init__(self):
        super().__init__()
        self.title("ООО «Лёгкий Шаг»")
        self.configure(bg=BG_MAIN)
        self.geometry("1150x650")
        self.minsize(900, 500)

        # ---- Верхняя шапка ----
        header = tk.Frame(self, bg=BG_SEC, padx=12, pady=6)
        header.pack(fill="x")
        tk.Label(header, text="ООО «Лёгкий Шаг»",
                 font=FONT_H, bg=BG_SEC).pack(side="left")
        tk.Label(header, text=f"{current_user['login']}  [{current_user['role']}]",
                 font=FONT, bg=BG_SEC).pack(side="right")

        # ---- Боковая панель навигации ----
        nav = tk.Frame(self, bg=BG_SEC, width=170, padx=6, pady=10)
        nav.pack(side="left", fill="y")
        nav.pack_propagate(False)

        role = current_user["role"]

        nav_btn_kw = dict(font=FONT_BOLD, width=16, anchor="w",
                          bg=BG_ACCENT, activebackground=BG_ACCENT,
                          relief="flat", pady=6, padx=4)

        tk.Button(nav, text="📦  Товары",
                  command=self.show_products, **nav_btn_kw).pack(pady=4, fill="x")

        if role in ("Manager", "Admin"):
            tk.Button(nav, text="📋  Заказы",
                      command=self.show_orders, **nav_btn_kw).pack(pady=4, fill="x")

        tk.Button(nav, text="🚪  Выйти",
                  command=self.logout, font=FONT_BOLD, width=16,
                  anchor="w", bg="#FF6B6B", activebackground="#FF6B6B",
                  relief="flat", pady=6, padx=4).pack(side="bottom", pady=4, fill="x")

        # ---- Область контента ----
        self.content = tk.Frame(self, bg=BG_MAIN)
        self.content.pack(side="left", fill="both", expand=True, padx=8, pady=8)

        self.show_products()

    def _clear(self):
        for w in self.content.winfo_children():
            w.destroy()

    def show_products(self):
        self._clear()
        ProductsPanel(self.content).pack(fill="both", expand=True)

    def show_orders(self):
        self._clear()
        OrdersPanel(self.content).pack(fill="both", expand=True)

    def logout(self):
        self.destroy()
        LoginForm().mainloop()

# ================================================================
#  ПАНЕЛЬ ТОВАРОВ
# ================================================================
class ProductsPanel(tk.Frame):
    COLS = [
        ("ID",          "id",       60),
        ("Название",    "name",    180),
        ("Артикул",     "article", 100),
        ("Цена",        "price",    90),
        ("Размер",      "size",     70),
        ("Бренд",       "brand",   110),
        ("Кол-во",      "quantity", 70),
        ("Скидка %",    "discount", 75),
    ]

    def __init__(self, parent):
        super().__init__(parent, bg=BG_MAIN)
        role = current_user["role"]

        tk.Label(self, text="Каталог товаров",
                 font=FONT_H, bg=BG_SEC, pady=6).pack(fill="x")

        # ── Поиск + фильтры (только Manager и Admin) ──────────────
        self._search_var = tk.StringVar()
        self._brand_var  = tk.StringVar(value="Все")
        self._sort_var   = tk.StringVar(value="Без сортировки")

        if role in ("Manager", "Admin"):
            ff = tk.Frame(self, bg=BG_MAIN, pady=4)
            ff.pack(fill="x")

            tk.Label(ff, text="Поиск:", font=FONT, bg=BG_MAIN).pack(side="left")
            e = tk.Entry(ff, textvariable=self._search_var, font=FONT, width=22)
            e.pack(side="left", padx=4)
            e.bind("<KeyRelease>", lambda _: self._load())

            tk.Label(ff, text="Бренд:", font=FONT, bg=BG_MAIN).pack(side="left", padx=(12, 0))
            self._brand_cb = ttk.Combobox(ff, textvariable=self._brand_var,
                                          state="readonly", font=FONT, width=13)
            self._brand_cb.pack(side="left", padx=4)
            self._brand_cb.bind("<<ComboboxSelected>>", lambda _: self._load())

            tk.Label(ff, text="Сортировка:", font=FONT, bg=BG_MAIN).pack(side="left", padx=(12, 0))
            sort_cb = ttk.Combobox(ff, textvariable=self._sort_var, state="readonly",
                                   font=FONT, width=18,
                                   values=["Без сортировки",
                                           "Цена ↑", "Цена ↓",
                                           "Название А→Я", "Название Я→А"])
            sort_cb.pack(side="left", padx=4)
            sort_cb.bind("<<ComboboxSelected>>", lambda _: self._load())

        # ── Кнопки CRUD (только Admin) ─────────────────────────────
        if role == "Admin":
            bf = tk.Frame(self, bg=BG_MAIN, pady=3)
            bf.pack(fill="x")
            accent_btn(bf, "➕ Добавить",  self._add).pack(side="left", padx=3)
            accent_btn(bf, "✏️ Изменить",  self._edit).pack(side="left", padx=3)
            accent_btn(bf, "🗑️ Удалить",  self._delete, color="#FF6B6B").pack(side="left", padx=3)

        # ── Таблица ────────────────────────────────────────────────
        tf = tk.Frame(self)
        tf.pack(fill="both", expand=True)

        cols = [c[0] for c in self.COLS]
        self.tree = ttk.Treeview(tf, columns=cols, show="headings", selectmode="browse")
        for label, _, width in self.COLS:
            self.tree.heading(label, text=label)
            self.tree.column(label, width=width, anchor="center")

        vsb = ttk.Scrollbar(tf, orient="vertical",   command=self.tree.yview)
        hsb = ttk.Scrollbar(tf, orient="horizontal",  command=self.tree.xview)
        self.tree.configure(yscrollcommand=vsb.set, xscrollcommand=hsb.set)

        vsb.pack(side="right",  fill="y")
        hsb.pack(side="bottom", fill="x")
        self.tree.pack(fill="both", expand=True)

        # Строки со скидкой > 15%
        self.tree.tag_configure("big_disc", background=BG_DISC, foreground=FG_DISC)

        self._load()
        self._fill_brands()

    def _fill_brands(self):
        conn = get_conn()
        brands = [r[0] for r in conn.execute(
            "SELECT DISTINCT brand FROM products ORDER BY brand")]
        conn.close()
        if hasattr(self, "_brand_cb"):
            self._brand_cb["values"] = ["Все"] + brands

    def _load(self):
        self.tree.delete(*self.tree.get_children())

        sql    = "SELECT id,name,article,price,size,brand,quantity,discount FROM products WHERE 1=1"
        params = []

        q = self._search_var.get().strip()
        if q:
            sql += " AND (name LIKE ? OR article LIKE ?)"
            params += [f"%{q}%", f"%{q}%"]

        brand = self._brand_var.get()
        if brand and brand != "Все":
            sql += " AND brand=?"
            params.append(brand)

        order_map = {
            "Цена ↑":       "price ASC",
            "Цена ↓":       "price DESC",
            "Название А→Я": "name ASC",
            "Название Я→А": "name DESC",
        }
        sort_key = order_map.get(self._sort_var.get(), "id ASC")
        sql += f" ORDER BY {sort_key}"

        conn = get_conn()
        for row in conn.execute(sql, params).fetchall():
            tag = ("big_disc",) if row["discount"] > 15 else ()
            self.tree.insert("", "end",
                             values=tuple(row),
                             tags=tag)
        conn.close()

    def _selected_id(self):
        sel = self.tree.selection()
        if not sel:
            messagebox.showwarning("Внимание", "Выберите строку в таблице")
            return None
        return self.tree.item(sel[0])["values"][0]

    def _add(self):
        ProductDialog(self).show()
        self._load(); self._fill_brands()

    def _edit(self):
        pid = self._selected_id()
        if pid is None: return
        ProductDialog(self, product_id=pid).show()
        self._load(); self._fill_brands()

    def _delete(self):
        pid = self._selected_id()
        if pid is None: return
        if messagebox.askyesno("Подтверждение", "Удалить выбранный товар?"):
            conn = get_conn()
            conn.execute("DELETE FROM products WHERE id=?", (pid,))
            conn.commit()
            conn.close()
            self._load(); self._fill_brands()

# ================================================================
#  ДИАЛОГ ДОБАВЛЕНИЯ / РЕДАКТИРОВАНИЯ ТОВАРА
# ================================================================
class ProductDialog(tk.Toplevel):
    # (метка, ключ_поля, тип, min_value_или_None)
    FIELDS = [
        ("Название",  "name",     "str",   None),
        ("Артикул",   "article",  "str",   None),
        ("Цена",      "price",    "float", 0.01),
        ("Размер",    "size",     "float", 1.0),
        ("Бренд",     "brand",    "str",   None),
        ("Кол-во",    "quantity", "int",   0),
        ("Скидка %",  "discount", "float", 0.0),
    ]

    def __init__(self, parent, product_id=None):
        super().__init__(parent)
        self.title("Добавить товар" if not product_id else "Изменить товар")
        self.configure(bg=BG_MAIN)
        self.resizable(False, False)
        self.product_id = product_id
        self._vars = {}

        tk.Label(self, text=self.title(), font=FONT_H, bg=BG_SEC, pady=6).pack(fill="x")

        body = tk.Frame(self, bg=BG_MAIN, padx=30, pady=10)
        body.pack()

        for i, (label, key, _, _) in enumerate(self.FIELDS):
            tk.Label(body, text=label + ":", font=FONT, bg=BG_MAIN, anchor="w",
                     width=10).grid(row=i, column=0, sticky="w", pady=4)
            v = tk.StringVar()
            tk.Entry(body, textvariable=v, font=FONT, width=24).grid(
                row=i, column=1, pady=4, padx=(8, 0))
            self._vars[key] = v

        if product_id:
            conn = get_conn()
            row = conn.execute(
                "SELECT name,article,price,size,brand,quantity,discount FROM products WHERE id=?",
                (product_id,)).fetchone()
            conn.close()
            if row:
                for (_, key, _, _) in self.FIELDS:
                    self._vars[key].set(row[key])

        accent_btn(body, "💾 Сохранить", self._save).grid(
            row=len(self.FIELDS), column=0, columnspan=2, pady=12)

    def show(self):
        self.grab_set()
        self.wait_window()

    def _save(self):
        data = {}
        for label, key, typ, min_val in self.FIELDS:
            raw = self._vars[key].get().strip()
            if not raw:
                messagebox.showerror("Ошибка валидации", f"Поле «{label}» не может быть пустым")
                return
            try:
                if typ == "float":
                    val = float(raw.replace(",", "."))
                elif typ == "int":
                    val = int(raw)
                else:
                    val = raw
            except ValueError:
                messagebox.showerror("Ошибка валидации", f"«{label}»: ожидается число")
                return
            if min_val is not None and isinstance(val, (int, float)) and val < min_val:
                messagebox.showerror("Ошибка валидации",
                                     f"«{label}» должно быть ≥ {min_val}")
                return
            data[key] = val

        conn = get_conn()
        try:
            if self.product_id:
                conn.execute("""
                    UPDATE products
                    SET name=:name, article=:article, price=:price,
                        size=:size, brand=:brand, quantity=:quantity, discount=:discount
                    WHERE id=:pid
                """, {**data, "pid": self.product_id})
            else:
                conn.execute("""
                    INSERT INTO products(name,article,price,size,brand,quantity,discount)
                    VALUES(:name,:article,:price,:size,:brand,:quantity,:discount)
                """, data)
            conn.commit()
            self.destroy()
        except sqlite3.IntegrityError:
            messagebox.showerror("Ошибка", "Артикул уже существует в базе данных")
        finally:
            conn.close()

# ================================================================
#  ПАНЕЛЬ ЗАКАЗОВ
# ================================================================
class OrdersPanel(tk.Frame):
    COLS = [
        ("ID",           "id",           60),
        ("Пользователь", "login",       130),
        ("Дата",         "date",        110),
        ("Статус",       "status",      130),
        ("Сумма (₽)",    "total_amount", 110),
    ]

    def __init__(self, parent):
        super().__init__(parent, bg=BG_MAIN)
        role = current_user["role"]

        tk.Label(self, text="Управление заказами",
                 font=FONT_H, bg=BG_SEC, pady=6).pack(fill="x")

        if role == "Admin":
            bf = tk.Frame(self, bg=BG_MAIN, pady=3)
            bf.pack(fill="x")
            accent_btn(bf, "➕ Создать",        self._add).pack(side="left",  padx=3)
            accent_btn(bf, "✏️ Изменить статус", self._edit).pack(side="left", padx=3)
            accent_btn(bf, "🗑️ Удалить",         self._delete,
                       color="#FF6B6B").pack(side="left", padx=3)

        tf = tk.Frame(self)
        tf.pack(fill="both", expand=True)

        cols = [c[0] for c in self.COLS]
        self.tree = ttk.Treeview(tf, columns=cols, show="headings", selectmode="browse")
        for label, _, width in self.COLS:
            self.tree.heading(label, text=label)
            self.tree.column(label, width=width, anchor="center")

        vsb = ttk.Scrollbar(tf, orient="vertical", command=self.tree.yview)
        self.tree.configure(yscrollcommand=vsb.set)
        vsb.pack(side="right", fill="y")
        self.tree.pack(fill="both", expand=True)

        self._load()

    def _load(self):
        self.tree.delete(*self.tree.get_children())
        conn = get_conn()
        rows = conn.execute("""
            SELECT o.id, COALESCE(u.login,'Гость') AS login,
                   o.date, o.status, o.total_amount
            FROM orders o
            LEFT JOIN users u ON o.user_id = u.id
            ORDER BY o.id DESC
        """).fetchall()
        conn.close()
        for row in rows:
            self.tree.insert("", "end", values=tuple(row))

    def _selected_id(self):
        sel = self.tree.selection()
        if not sel:
            messagebox.showwarning("Внимание", "Выберите заказ")
            return None
        return self.tree.item(sel[0])["values"][0]

    def _add(self):
        OrderDialog(self).show()
        self._load()

    def _edit(self):
        oid = self._selected_id()
        if oid is None: return
        OrderDialog(self, order_id=oid).show()
        self._load()

    def _delete(self):
        oid = self._selected_id()
        if oid is None: return
        if messagebox.askyesno("Подтверждение", "Удалить этот заказ?"):
            conn = get_conn()
            conn.execute("DELETE FROM order_items WHERE order_id=?", (oid,))
            conn.execute("DELETE FROM orders WHERE id=?", (oid,))
            conn.commit()
            conn.close()
            self._load()

# ================================================================
#  ДИАЛОГ ЗАКАЗА
# ================================================================
STATUSES = ["Новый", "В обработке", "Отправлен", "Доставлен", "Отменён"]

class OrderDialog(tk.Toplevel):
    def __init__(self, parent, order_id=None):
        super().__init__(parent)
        self.title("Новый заказ" if not order_id else f"Заказ #{order_id}")
        self.configure(bg=BG_MAIN)
        self.resizable(False, False)
        self.order_id = order_id

        tk.Label(self, text=self.title(), font=FONT_H, bg=BG_SEC, pady=6).pack(fill="x")

        body = tk.Frame(self, bg=BG_MAIN, padx=30, pady=14)
        body.pack()

        tk.Label(body, text="Статус:", font=FONT, bg=BG_MAIN, anchor="w",
                 width=10).grid(row=0, column=0, sticky="w", pady=6)
        self._status = tk.StringVar(value="Новый")
        ttk.Combobox(body, textvariable=self._status, values=STATUSES,
                     state="readonly", font=FONT, width=22).grid(row=0, column=1, padx=(8, 0))

        tk.Label(body, text="Сумма:", font=FONT, bg=BG_MAIN, anchor="w",
                 width=10).grid(row=1, column=0, sticky="w", pady=6)
        self._total = tk.StringVar(value="0.00")
        tk.Entry(body, textvariable=self._total, font=FONT, width=24).grid(
            row=1, column=1, padx=(8, 0))

        if order_id:
            conn = get_conn()
            row = conn.execute(
                "SELECT status, total_amount FROM orders WHERE id=?",
                (order_id,)).fetchone()
            conn.close()
            if row:
                self._status.set(row["status"])
                self._total.set(row["total_amount"])

        accent_btn(body, "💾 Сохранить", self._save).grid(
            row=2, column=0, columnspan=2, pady=12)

    def show(self):
        self.grab_set()
        self.wait_window()

    def _save(self):
        status = self._status.get()
        try:
            total = float(self._total.get().replace(",", "."))
            if total < 0:
                raise ValueError
        except ValueError:
            messagebox.showerror("Ошибка", "Сумма должна быть числом ≥ 0")
            return

        conn = get_conn()
        today = datetime.date.today().isoformat()
        if self.order_id:
            conn.execute("UPDATE orders SET status=?, total_amount=? WHERE id=?",
                         (status, total, self.order_id))
        else:
            conn.execute(
                "INSERT INTO orders(user_id, date, status, total_amount) VALUES(?,?,?,?)",
                (current_user["id"], today, status, total))
        conn.commit()
        conn.close()
        self.destroy()

# ================================================================
#  ТОЧКА ВХОДА
# ================================================================
if __name__ == "__main__":
    init_db()
    LoginForm().mainloop()
