<template>
  <div class="auth-layout">
    <header class="auth-header">
      <!-- AuthLayout 헤더 내용 (예: 로고) -->
      <h2>인증 레이아웃</h2>
    </header>
    <main class="auth-content">
      <router-view /> <!-- 여기에 LoginView.vue 또는 RegisterView.vue 등이 렌더링됩니다. -->
    </main>
    <footer class="auth-footer">
      <p>&copy; 2023 My App Auth</p>
    </footer>
  </div>
</template>

<script setup>
// 필요한 스크립트 로직
</script>

<style scoped>
.auth-layout { display: flex; flex-direction: column; align-items: center; justify-content: center; min-height: 100vh; background-color: #f4f4f4; }
.auth-header, .auth-footer { padding: 1rem; text-align: center; }
.auth-content { background-color: white; padding: 2rem; border-radius: 8px; box-shadow: 0 0 10px rgba(0,0,0,0.1); }
</style>