import requests
import threading
from kivy.app import App
from kivy.lang import Builder
from kivy.uix.screenmanager import ScreenManager, Screen
from kivy.uix.label import Label
from kivy.uix.button import Button
from kivy.uix.boxlayout import BoxLayout
from kivy.clock import Clock
from kivy.graphics import Color, RoundedRectangle

KV = '''
ScreenManager:
    LoginScreen:
    DashboardScreen:

<LoginScreen>:
    name: 'login'
    BoxLayout:
        orientation: 'vertical'
        padding: 40
        spacing: 20
        canvas.before:
            Color:
                rgba: 0.06, 0.09, 0.16, 1
            Rectangle:
                pos: self.pos
                size: self.size
        Label:
            text: 'SMART [color=38bdf8]GYM[/color]'
            markup: True
            font_size: '40sp'
            bold: True
        TextInput:
            id: username
            hint_text: 'Username'
            size_hint_y: None
            height: '50dp'
            multiline: False
            write_tab: False
        TextInput:
            id: password
            hint_text: 'Password'
            password: True
            size_hint_y: None
            height: '50dp'
            multiline: False
            write_tab: False
        Button:
            id: login_btn
            text: 'ĐĂNG NHẬP'
            size_hint_y: None
            height: '55dp'
            bold: True
            background_color: (0.22, 0.74, 0.97, 1)
            on_press: root.login_thread()

<DashboardScreen>:
    name: 'dashboard'
    BoxLayout:
        orientation: 'vertical'
        canvas.before:
            Color:
                rgba: 0.06, 0.09, 0.16, 1
            Rectangle:
                pos: self.pos
                size: self.size

        BoxLayout:
            size_hint_y: 0.12
            padding: 10
            canvas.before:
                Color:
                    rgba: 0.15, 0.2, 0.3, 1
                Rectangle:
                    pos: self.pos
                    size: self.size
            Label:
                id: welcome_text
                text: 'Hệ thống Smart Gym'
                bold: True
                font_size: '18sp'
            Button:
                text: 'Thoát'
                size_hint_x: 0.25
                bold: True
                background_color: (0.8, 0.2, 0.2, 1)
                on_press: root.logout()

        ScrollView:
            do_scroll_x: False
            BoxLayout:
                id: container
                orientation: 'vertical'
                padding: 15
                spacing: 15
                size_hint_y: None
                height: self.minimum_height
'''

API_URL = "https://api.thanhchinh.io.vn/api"

class LoginScreen(Screen):
    def login_thread(self):
        self.ids.login_btn.disabled = True
        self.ids.login_btn.text = "Đang kết nối..."
        threading.Thread(target=self.do_login, daemon=True).start()

    def do_login(self):
        u = self.ids.username.text.strip()
        p = self.ids.password.text.strip()
        try:
            res = requests.post(f"{API_URL}/Auth/login", json={"username": u, "password": p}, timeout=5)
            if res.status_code == 200:
                data = res.json()
                Clock.schedule_once(lambda dt: self.go_to_dashboard(data))
            else:
                Clock.schedule_once(lambda dt: self.update_status("Sai tài khoản!"))
        except:
            Clock.schedule_once(lambda dt: self.update_status("Server Offline!"))

    def update_status(self, msg):
        self.ids.login_btn.disabled = False
        self.ids.login_btn.text = msg
        Clock.schedule_once(lambda dt: setattr(self.ids.login_btn, 'text', 'ĐĂNG NHẬP'), 2)

    def go_to_dashboard(self, data):
        self.ids.login_btn.disabled = False
        self.ids.login_btn.text = "ĐĂNG NHẬP"
        dash = self.manager.get_screen('dashboard')
        dash.user_data = data
        dash.init_dashboard()
        self.manager.current = 'dashboard'

class DashboardScreen(Screen):
    user_data = {}
    is_fetching = False

    def init_dashboard(self):
        self.ids.container.clear_widgets()
        role = str(self.user_data.get('Role') or self.user_data.get('role', 'Member')).upper()
        name = self.user_data.get('FullName') or self.user_data.get('fullName', 'User')
        self.ids.welcome_text.text = f"{role}: {name}"

        if role == "PT":
            Clock.unschedule(self.fetch_realtime_logs)
            Clock.schedule_interval(self.fetch_realtime_logs, 2)
        else:
            self.add_info_card(f"Chào mừng hội viên: {name}", (0.22, 0.74, 0.97, 0.4))

    def fetch_realtime_logs(self, dt):
        if self.is_fetching: return
        self.is_fetching = True
        threading.Thread(target=self.get_api_data, daemon=True).start()

    def get_api_data(self):
        try:
            res = requests.get(f"{API_URL}/Rfid/get-new-logs", timeout=2)
            if res.status_code == 200:
                new_data = res.json()
                if new_data and isinstance(new_data, list):
                    Clock.schedule_once(lambda dt: self.append_cards(new_data))
        except Exception as e:
            print(f"API Error: {e}")
        finally:
            self.is_fetching = False

    def append_cards(self, data_list):
        for item in data_list:
            h_id = item.get('Id') or item.get('id')
            u_name = item.get('UserName') or item.get('userName', 'Thẻ lạ')
            status = item.get('Status') or item.get('status', 'N/A')
            time_str = item.get('CheckInTime') or item.get('checkInTime', '')

            card = BoxLayout(orientation='vertical', size_hint_y=None, height='140dp', padding=15, spacing=8)

            lbl = Label(
                text=f"[b]HỘI VIÊN:[/b] {u_name}\n[size=14sp]{time_str} - {status}[/size]",
                markup=True, halign='center', color=(1, 1, 1, 1)
            )
            lbl.bind(size=lbl.setter('text_size'))

            btn = Button(
                text="NHẬN HỘI VIÊN", size_hint_y=None, height='45dp',
                bold=True, background_normal='', background_color=(0.1, 0.6, 0.1, 1)
            )
            btn.bind(on_press=lambda x, c=card, idx=h_id: self.confirm_and_remove(c, idx))

            card.add_widget(lbl)
            card.add_widget(btn)

            with card.canvas.before:
                Color(0.2, 0.25, 0.35, 1)
                card.bg_rect = RoundedRectangle(pos=card.pos, size=card.size, radius=[12])

            card.bind(pos=lambda inst, pos: setattr(inst.bg_rect, 'pos', pos),
                      size=lambda inst, size: setattr(inst.bg_rect, 'size', size))

            self.ids.container.add_widget(card, index=len(self.ids.container.children))

    def confirm_and_remove(self, card_widget, history_id):
        if history_id:
            threading.Thread(target=self.send_confirm, args=(history_id,), daemon=True).start()
        self.ids.container.remove_widget(card_widget)

    def send_confirm(self, history_id):
        try:
            requests.post(f"{API_URL}/Rfid/confirm/{history_id}", timeout=2)
        except: pass

    def add_info_card(self, text, color):
        lbl = Label(text=text, size_hint_y=None, height='100dp', bold=True, halign='center')
        lbl.bind(size=lbl.setter('text_size'))
        with lbl.canvas.before:
            Color(*color)
            lbl.bg_rect = RoundedRectangle(pos=lbl.pos, size=lbl.size, radius=[15])
        lbl.bind(pos=lambda inst, pos: setattr(inst.bg_rect, 'pos', pos),
                 size=lambda inst, size: setattr(inst.bg_rect, 'size', size))
        self.ids.container.add_widget(lbl)

    def logout(self):
        Clock.unschedule(self.fetch_realtime_logs)
        self.manager.current = 'login'

class SmartGymApp(App):
    def build(self):
        self.title = "Smart Gym - PT Dashboard"
        return Builder.load_string(KV)

if __name__ == '__main__':
    SmartGymApp().run()