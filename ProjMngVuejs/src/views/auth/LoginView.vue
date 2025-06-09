<template>
  <div>
    <h3>로그인 페이지</h3>
    <form @submit.prevent="handleLogin">
      <!-- 로그인 폼 요소들 -->
      <button type="submit">로그인</button>
    </form>
    <p>계정이 없으신가요? <router-link to="/register">회원가입</router-link></p>
  </div>
</template>

<script setup>
const handleLogin = () => { console.log('로그인 시도'); };
</script>