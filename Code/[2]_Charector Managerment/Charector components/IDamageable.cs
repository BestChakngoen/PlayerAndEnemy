// ระบบ: Combat System (Interface)
// หน้าที่: กำหนดมาตรฐานให้ทุก Object ที่มีเลือดและรับดาเมจได้ต้องมีฟังก์ชันนี้
// (SOLID: Dependency Inversion Principle)
public interface IDamageable
{
    void TakeDamage(float damageAmount);
}