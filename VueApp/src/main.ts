import './assets/bootstrap/bootstrap.min.css'
import './assets/site.css'

import { createApp } from 'vue'
import App from './App.vue'
import router from './router'

router.afterEach((to) => {
  const defaultTitle = 'Vue App';
  (document as any).title = to.meta?.title || defaultTitle;
});

const app = createApp(App)

app.use(router)

app.mount('#app')
